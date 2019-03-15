begin

declare local temporary table TMP_WORK(
	TMP_IDX int identity not null
	,entity_type	int
	,entity_ref		int
	,entity_table	int
	,update_def		int
) on commit preserve rows;


insert into TMP_WORK(entity_type,entity_ref,entity_table,update_def)
SELECT entity_type,entity_Ref,entity_table,update_def
FROM [ETL_LOG_CUD] with (nolock)
where entity_type = 9 and  update_type = 'D';



update [ETL_LOG_CUD]
set update_type = (case update_type
	when 'D' then '3'
    else update_type
    end)
where update_def in (select update_Def from TMP_WORK);



SELECT 
  t.entity_ref as [contact_def]
,t.entity_table as [resident_ref]
FROM TMP_WORK t
left join [CONTACTS] c with (nolock) on c.contact_def = t.entity_ref
order by c.contact_def asc;

end