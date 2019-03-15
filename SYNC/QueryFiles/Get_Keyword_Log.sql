begin

declare local temporary table TMP_WORK(
	TMP_IDX int identity not null
	,entity_type	int
	,entity_ref		int
	,entity_table	varchar(1000)
	,update_def		int
) on commit preserve rows;


insert into TMP_WORK(entity_type,entity_ref,entity_table,update_def)
SELECT entity_type,entity_Ref,entity_table,update_def
FROM [ETL_LOG_CUD] with (nolock)
where entity_type = 99
and (update_type = 'U' or update_type = 'C' or update_type = 'D') 
and entity_table not like '%KEYWORD_REF:100001156%'
order by entity_ref asc;


update [ETL_LOG_CUD]
set update_type = (case update_type
    when 'C' then '1'
    when 'U' then '2'
    when 'D' then '3'
   else update_type
     end)
where update_def in (
select update_def from TMP_WORK);

SELECT 
	resident_ref
	,keyword_no
	,keyword_ref
	,keyword_text
FROM KEYWORD_RELATION
WHERE RESIDENT_REF IN(
SELECT DISTINCT entity_Ref
FROM TMP_WORK)
ORDER BY KEYWORD_NO;

end