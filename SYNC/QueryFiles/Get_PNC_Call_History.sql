begin

select 
    ch.call_def
    ,ch.arrival_time
    ,case ch.call_entity_type
        when 2 then 'Home'
        else 'Unknown'
     end as [Type]
    ,el.location_def
    ,el.Equip_Id
    ,el.equip_phone
    ,ch.call_code
    ,case ch.Call_Info
        when 4002 then (select resident_def from Resident where location_ref = el.location_def  and primary_YN = 'Y')
        else (select resident_def from Resident where location_ref = el.location_def and primary_YN = 'Y')
    end [Resident_Def]
    ,ch.Call_info
    ,ch.meaning
    ,ch.protocol_tag
    ,r.reason_def
    ,r.text as [reason]
from Calls_HISTORY ch
join EPEC_LOCATION el on ch.call_entity_ref = el.location_def
join REASONS r on ch.Reason_ref = r.reason_def
where arrival_time between dateadd(dd,-1,getdate()) and getdate()
and call_entity_type = 2
and ch.call_code not in (' P')
and (ch.protocol_tag = 'AM' or ch.incoming_yn = 'N' and ch.Protocol_tag = 'IT' and ch.call_code = ' X')
order by arrival_time desc;

end