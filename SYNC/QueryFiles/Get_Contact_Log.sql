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
FROM ETL_LOG_CUD
where (update_type = 'U' or update_type = 'C') 
and entity_type = 8;

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


SELECT 
    c.contact_def
,c.contact_type_ref
,c.first_name +' ' + c.last_name as [ResponderName]
,isnull(case substring(c.s_phone_1,1,1)
    when '1' then substring(c.s_phone_1,2,10)    
    else c.s_phone_1
end,'') as [HomePhone]
,isnull(case substring(c.s_phone_2,1,1)
    when '1' then substring(c.s_phone_2,2,10)
    else c.s_phone_2
end,'') as [WorkPhone]
,isnull(case substring(c.s_phone_3,1,1)
    when '1' then substring(c.s_phone_3,2,10)
    else c.s_phone_3
end,'') as [CellPhone]
,cr.keyholder_yn as [HasKey]
,case isnull(c.email_address,0)
    when 0 then 'N' 
    else 'Y'  
end as [NVI]
,cast(case isnull(c.email_address,0)
     when 0 then 0
	else 
		CASE
			when c.email_address like '1[0-9][0-9]' then c.email_address - 99
            when c.email_address like '[0-9]' then c.email_address
			else 0
		end
end as int) as [NVIOrder]
,case 	
		when cr.Priority like '1[0-9][0-9]' then cr.Priority - 99
		when cr.Priority like '2[0-9][0-9]' then cr.Priority - 199
		when cr.Priority like '101[0-9][0-9]' then cr.Priority - 10099
		when cr.Priority like '104[0-9][0-9]' then cr.Priority - 10399
		when cr.Priority in ('11','13') then Row_Number()Over(partition by cr.resident_ref order by cr.relation_def)
	else
		cr.Priority
	end as [Priority]
,cr.relation_def
,cr.location_ref
,cr.resident_ref
,t.entity_type
FROM TMP_Final t
inner join CONTACT_RELATION cr on cr.contact_ref = t.entity_ref
inner join CONTACTS c on c.contact_def = cr.contact_ref;

end