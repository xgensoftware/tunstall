begin

declare local temporary table TMP_WORK(
	TMP_IDX int identity not null
	,entity_type	int
	,entity_ref		int
	,update_def		int
	,entity_table	varchar(1000)
) on commit preserve rows;


insert into TMP_WORK(entity_type,entity_ref,update_def,entity_table)
SELECT entity_type,entity_Ref, update_def,entity_table
FROM [ETL_LOG_CUD] with (nolock)
where entity_type = 97 and  update_type = 'D';

update [ETL_LOG_CUD]
set update_type = (case update_type
	when 'D' then '3'
    else update_type
    end)
where update_def in (
select update_def from TMP_WORK);


select 
    entity_type
    ,entity_ref
    ,update_def
    ,entity_table
from TMP_WORK;

end