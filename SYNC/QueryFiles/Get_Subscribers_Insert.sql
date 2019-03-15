declare @TempLog table (
	ID					bigint
	,Subscriber_Id		varchar(25)
	,Update_Type		char(1)
)


insert into @TempLog(ID,Subscriber_Id,Update_Type)
SELECT top 500
      ID
      ,TABLE_ID
      ,UPDATE_TYPE      
  FROM [KS_EtlChangeLog]  
  where is_Processed = 0  
  and Table_name = 'Subscriber'
  and Update_Type = 'I'
  order by id asc 
  
select distinct
   s.[Subscriber_ID] 
  ,s.[FirstName]
  ,s.[Agency_ID]
  ,s.[MiddleInitial]
  ,s.[LastName]
  ,s.DOB
  ,s.[Agency_ID]
  ,s.[Address1]
  ,s.[Address2]
  ,CASE 
    WHEN  s.[ZipPlus4] IN('    ', NULL,'') THEN  s.[Zip] 
     ELSE  s.[Zip] + '-' + s.[ZipPlus4]
   END AS ZIP
  ,s.[City]
  ,s.[State]
  ,CASE WHEN LEN(s.[Phone]) = 10 THEN '1' + s.[Phone]
   ELSE s.[Phone]
   END AS phone
  ,s.[InstallDate]  
  ,'Y' As SS_RESTRICT_YN
  ,a.[AUTHORITY_REF]
  ,CASE WHEN S.[STATUS] = 'ACTIVE' and a.[AUTHORITY_REF] in (100000006,100000029,100000080,100000081) then C.[ALTERNATE_COLOUR]
   ELSE c.[COLOUR_REF]
   END AS COLOUR_REF
  ,s.RemovalDate As TerminationDate
  ,s.[OnlineSince] As ActivationDate
  ,case 
  when isnull(i.NoLandlineAvailable,'No') = 'Yes' and isnull(i.RequestMSD,'No') <> 'Yes' then 100000205  
  when isnull(i.NoLandlineAvailable,'No') <> 'Yes' and isnull(i.RequestMSD,'No') = 'Yes' then 92
  else 150
  end as Equip_Model_Ref
  ,CASE  WHEN s.[Subscriber_ID] = s.[MasterReference]  then  'Y' 
  ELSE 'N' END AS PRIMARY_YN
  ,s.MasterReference
  ,CASE WHEN ISNULL(f.DoctorName,'') IN ('',' ', NULL) 
   AND ISNULL(f.DoctorPhone,'') IN ('',' ', NULL) 
   AND ISNULL(f.Hospital_ID,'') IN ('',' ', NULL)THEN 'N'
   ELSE 'Y'
   END AS Resident_Notes_YN  
   ,CASE WHEN s.[NearIntersection] IN (' ', NULL) THEN 'N'
   ELSE 'Y'
  END AS Epec_Notes_YN
  , s.[NearIntersection] 
  ,case 
	when isnull(i.NoLandlineAvailable,'No') <> 'Yes' and isnull(i.RequestMSD,'No') = 'Yes' then i.MSDPhone
	else '' 
  end AS OTHER_PHONE
  ,s.[Status]
  ,(select top 1 Language_Id
	from [dbo].[KS_SubscriberLanguage] where Subscriber_ID = s.Subscriber_ID) as [Language_ID]
   ,s.Sex as [Gender]
   ,s.Type
   ,coalesce(i.PNC_IDENT,-1) as [Equip_Id]
   ,s.Deleted
   ,s.SecondUser
   ,s.IPAddress
from @TempLog l
join dbo.ks_Subscriber s with (nolock) on s.Subscriber_Id = l.Subscriber_Id
inner join [dbo].[MAP_AGENCY_AUTHORITY] a  with (nolock) on s.[Agency_ID] = a.AGENCY_ID 
inner join [dbo].[MAP_COLOUR_REF] c with (nolock) on s.status = c.[STATUS]
left outer join [dbo].[ks_SubscriberPL2info] i with (nolock) on s.subscriber_id = i.subscriber_id
LEFT OUTER JOIN dbo.KS_SubscriberERCInfo f WITH(NOLOCK) on  f.[Subscriber_ID] = s.[Subscriber_ID];

select * from @TempLog;
