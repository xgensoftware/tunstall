begin
declare local temporary table TMP_WORK(
	TMP_IDX int identity not null
	,entity_type	int
	,entity_ref		int
	,update_def		int
) on commit preserve rows;

insert into TMP_WORK(entity_type,entity_ref,update_def)
SELECT entity_type,entity_Ref,update_def
FROM [ETL_LOG_CUD] with (nolock)
where entity_type = 97 and  (update_type = 'U' or update_type = 'C');

update [ETL_LOG_CUD]
set update_type = (case update_type
	when 'C' then '1'
    when 'U' then '2'
    else update_type
    end)
where update_def in (select update_def from TMP_WORK);

SELECT 
 n.notes_def
,n.entity_type
,n.entity_ref as [note_entity]
,n.title
,n.[text]
FROM [NOTES] n with (nolock)
inner join TMP_WORK t on t.entity_ref = n.notes_def
where n.entity_type in(2,5,8)
order by n.entity_ref asc;

end