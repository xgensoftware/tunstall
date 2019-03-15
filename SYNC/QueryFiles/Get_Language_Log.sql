begin

declare local temporary table TMP_WORK(
	TMP_IDX int identity not null
	,update_def		int
	,entity_type	int
	,entity_ref		int
	,entity_table	varchar(255)
) on commit preserve rows;


insert into TMP_WORK(update_def,entity_type,entity_ref,entity_table)
SELECT
update_def
,case substring(entity_table,0,4)
 when 'RES' then 5
 else 2
end as [temp_entity_type]
,entity_Ref,entity_table
FROM [ETL_LOG_CUD] 
where entity_type = 98
and (update_type = 'U' or update_type = 'C') 
order by entity_ref asc ;



update [ETL_LOG_CUD]
set update_type = (case update_type
    when 'C' then '1'
    when 'U' then '2'
   else update_type
     end)
where update_def in (
select update_def
from TMP_WORK);


SELECT distinct
ad.entity_type
,ad.entity_ref
,ac.attr_category_def
,ac.text as [attr_category_text]
,atc.attr_choice_def
,atc.text as [attr_choice_text]
FROM TMP_WORK t
inner join [ATTR_DEF] ad with (nolock) on ad.entity_ref = t.entity_ref and ad.entity_type=t.entity_type
inner join [ATTR_CATEGORIES] ac with (nolock) on ac.attr_category_def = ad.attr_category_ref
inner join [ATTR_CHOICE] atc with (nolock) on atc.attr_choice_def = ad.attr_choice_ref
where ac.attr_category_def in(100000001,4);

end