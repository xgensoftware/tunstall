begin
declare local temporary table TMP_WORK(
	TMP_IDX int identity not null
	,entity_type	int
	,entity_ref		int
	,update_def		int
) on commit preserve rows;

declare local temporary table TMP_Final(
	entity_type		int
	,entity_ref		int
) on commit preserve rows;


insert into TMP_WORK(entity_type,entity_ref,update_def)
SELECT entity_type,entity_Ref,update_def
FROM [ETL_LOG_CUD] 
where (update_type = 'U'or update_type = 'C') 
and entity_type = 5;

insert into TMP_Final(entity_type,entity_ref)
select distinct
entity_type,entity_ref
from TMP_WORK;

update [ETL_LOG_CUD]
set update_type = (case update_type
    when 'C' then '1'
    when 'U' then '2'
    else update_type
   end)
where update_def in (
select update_def from TMP_WORK);


select distinct
	el.location_def
    ,el.equip_id as [unit_id]
    ,el.address_street as [Address1]
    ,el.address_area as [Address2]
    ,el.authority_ref as [agency_id]
	,el.authority_ref
    ,el.address_town as [City]
    ,SUBSTRING(UPPER(ISNULL(el.address_county,'')),1,2) as [State]
    ,substring(el.address_postcode,1,5) as [Zip]
	,CASE WHEN DATALENGTH(address_postcode) IN(9,10) THEN right(address_postcode,4)
		ELSE '    '
		END as [zipplus4] 
	,r.resident_def
	,(SELECT DISTINCT 
	 LIST(kr.keyword_text, ',')
	FROM [KEYWORD_RELATION] kr
	inner join [KEYWORDS] k  on k.keyword_def = kr.keyword_ref
	where k.text like 'Sp Instruction'
	and kr.resident_ref = r.resident_def and len(KR.KEYWORD_TEXT) > 0 ) as [specialinst]
	,r.date_of_birth as [dob]
	,r.date_time_created as [entrydate]
	,cast(r.first_name as varchar(20)) as [firstname]
	,cast(r.last_name as varchar(30)) as [lastname]
	,cast(r.middle_name as varchar(1)) as [middleinitial]
	,r.first_name + ' ' + r.last_name as [fullname]
	,r.Primary_YN
	,coalesce((SELECT top 1 ac.attr_choice_def
	FROM [ATTR_DEF] ad
	inner join [ATTR_CHOICE] ac  on ad.attr_choice_ref = ac.attr_choice_def
	where ac.attr_category_ref = 100000128 and ad.entity_type = 5 and ac.attr_choice_def in(100000294,100000310,100000292,100000293,100000297)
	and ad.entity_ref = r.resident_def),100000310) as [attr_choice_def]
	,(SELECT top 1 [text]
	FROM [NOTES]
	where entity_type = 2 and entity_ref = r.location_ref
	and Title = 'Directions To Home' and len([text]) > 0) as [NearestIntersection]
    ,substring(atc.text,0,2) as [gender]
    ,(select 
	    case count(location_ref) 
        when 1 then 'N'
        else 'Y' end
       from [RESIDENT]
        where resident_def = r.resident_def
       group by resident_def) as [SecondUser]
    ,el.installation_date as [installdate]
    ,case substring(el.s_equip_phone,1,1)
        when '1' then substring(el.s_equip_phone,2,10)
        else el.s_equip_phone
    end as [Phone]
    ,el.termination_date as [RemovalDate]
    ,el.Activation_date as [OnlineSince]
    ,el.colour_ref as equip_status
	,el.colour_ref
    ,el.equip_model_ref 
	,(select text from EQUIP_MODELS WHERE EQUIP_MODEL_DEF IN(el.equip_model_ref)) as [equip_model_text]
--,coalesce((select top 1 em.text
--    from EQUIPMENT e
--    left join EQUIP_MODELS em on e.equip_model_ref = em.equip_model_def
--    where e.ident = el.equip_id
--order by e.installation_date desc
--),(select text from EQUIP_MODELS WHERE EQUIP_MODEL_DEF IN(el.equip_model_ref)))
-- as [equip_model_text]	
    ,case substring(el.OTHER_PHONE,1,1)
        when '1' then substring(el.OTHER_PHONE,2,10)
        else el.OTHER_PHONE
    end as [Other_Phone]
    ,CASE 
	    WHEN el.equip_model_Ref in(100000205,61,100000262,100000379) then 'Cellular'
	    when el.Equip_Model_Ref in(92,100000380) THEN 'MSD'
	    --when el.Equip_Model_Ref  in(100000003, 100000004)  THEN 'DSPNET'
	 ELSE '(PNC)'
    END AS Type
	, (select top 1 d.attr_choice_ref
		from STANDARD.ATTR_DEF d
		where d.entity_type = 2 and d.ATTR_CATEGORY_REF = 100000003
		and d.entity_ref = el.Location_def) as [Sub_Type]
	,r.date_time_created as [OrderDate]
	,r.phone_2 as [Mobile]
from TMP_Final t
inner join [RESIDENT] r on r.resident_def = t.entity_ref
inner join EPEC_LOCATION el on el.location_def = r.location_ref
inner join [ATTR_DEF] ad on ad.entity_ref = r.resident_def
inner join [ATTR_CATEGORIES] ac  on ac.attr_category_def = ad.attr_category_ref
inner join [ATTR_CHOICE] atc on atc.attr_choice_def = ad.attr_choice_ref 
where (ac.attr_category_def = 306 and ad.entity_type = 5)
order by r.resident_def asc;

END