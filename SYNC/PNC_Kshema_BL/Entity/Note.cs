using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using com.Xgensoftware.Core;
using com.Xgensoftware.DAL;

namespace PNC_Kshema.BL.Entity
{
    class Note : EntityBase, IEntity
    {
        #region " Member Variables "
        int _notes_def;
        int _update_def;
        int _entity_type;
        int _note_entity;
        int _entity_ref;

        int _script_entity_type;
        int _script_entity_ref;
        string _script_title;

        string _note_title;
        string _note_text;

        #endregion

        #region " Constructor "
        public Note(DataRow dr, string group_record)
        {
            _dataToMapp = dr;
            this._group_record = group_record;

            //intialize the providers
            base.InitializeProviders();
        }
        #endregion

        #region " Private Methods "
        private int Parse_Hospital_Id()
        {
            string sPattern = @"\[(.*?)\]";
            int hospitalId = -1;

            try
            {
                Match match = Regex.Match(_note_text, sPattern, RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    StringBuilder id = new StringBuilder(match.Groups[0].Value);
                    id.Replace("[", "");
                    id.Replace("]", "");

                    int.TryParse(id.ToString(), out hospitalId);
                }
            }
            catch { }

            return hospitalId;
        }

        private void Process_Responder_Contact_Note()
        {
            try
            {
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@notes_def", _notes_def));
                _sqlParms.Add(SQLParam.GetParam("@note_entity", _note_entity));
                _sqlParms.Add(SQLParam.GetParam("@entity_type", _entity_type));
                _sqlParms.Add(SQLParam.GetParam("@note_title", _note_title));
                _sqlParms.Add(SQLParam.GetParam("@note_text", _note_text));
                _stageProvider.ExecuteNonQuery("SYNC_Process_Responder_Note", _sqlParms);
                this.EntityMessage(string.Format("Processed responder note for note_def {0}", _notes_def.ToString()),"INFO");
            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Unable to process responder notes for note_def: {0}, entity_type: {1}, note_entity: {2}", _notes_def.ToString(),_entity_type.ToString(),_note_entity.ToString()),"ERROR");
                
            }
        }

        private void _Process_Delete_Resonder_Contact_Note()
        {
            try
            {
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@entity_ref", _entity_ref));
                _stageProvider.ExecuteNonQuery("SYNC_Delete_ResponderContact_Note", _sqlParms);
                this.EntityMessage(string.Format("Processed responder/contact note for note_def {0}",_entity_ref.ToString()),"INFO");
            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Unable to delete responder/contact notes for note_def: {0}, entity_type: {1}. ERROR: {2}", _entity_ref.ToString(), _entity_type.ToString(),ex.Message),"ERROR");

            }
        }

        private void Process_Hospital_Note()
        {
            int hospitalid = this.Parse_Hospital_Id();
            if (hospitalid != -1)
            {
                try
                {
                    _sqlParms = new List<SQLParam>();
                    _sqlParms.Add(SQLParam.GetParam("@note_entity", _note_entity));
                    _sqlParms.Add(SQLParam.GetParam("@note_title", _note_title));
                    _sqlParms.Add(SQLParam.GetParam("@note_text", _note_text));
                    _sqlParms.Add(SQLParam.GetParam("@hospital_id",hospitalid));
                    _stageProvider.ExecuteNonQuery("SYNC_Process_Hospital_Note", _sqlParms);
                    this.EntityMessage(string.Format("Processed hospital note for note_def {0}", _notes_def.ToString()),"INFO");
                }
                catch (Exception ex)
                {
                    this.EntityMessage(string.Format("Unable to process hospital note for note_def: {0}, entity_type: {1}, note_entity: {2}", _notes_def.ToString(), _entity_type.ToString(), _note_entity.ToString()),"ERROR");

                }
            }
        }

        private void Delete_Hospital_Note()
        {
            try
            {
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@entity_ref", _script_entity_ref));
                _stageProvider.ExecuteNonQuery("SYNC_Delete_Hospital_Note", _sqlParms);
                this.EntityMessage(string.Format("Delted hospital note for resident_def {0}", _script_entity_ref),"INFO");
            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Unable to delete hospital note for resident_def: {0}. ERROR: {1}", _script_entity_ref.ToString(),ex.Message),"ERROR");

            }
        }

        private void Process_Doctor_Note()
        {
            int hospitalid = this.Parse_Hospital_Id();
            if (hospitalid != -1)
            {
                try
                {
                    _sqlParms = new List<SQLParam>();
                    _sqlParms.Add(SQLParam.GetParam("@note_entity", _note_entity));
                    _sqlParms.Add(SQLParam.GetParam("@note_title", _note_title));
                    _sqlParms.Add(SQLParam.GetParam("@note_text", _note_text));
                    _stageProvider.ExecuteNonQuery("SYNC_Process_Doctor_Note ", _sqlParms);
                    this.EntityMessage(string.Format("Processed doctor note for note_def {0}", _notes_def.ToString()),"INFO");
                }
                catch (Exception ex)
                {
                    this.EntityMessage(string.Format("Unable to process doctor note for note_def: {0}, entity_type: {1}, note_entity: {2}", _notes_def.ToString(), _entity_type.ToString(), _note_entity.ToString()),"ERROR");

                }
            }
        }
                
        private void Process_Resident_Epec_Directions()
        {
            try
            {
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@note_entity", _note_entity));
                _sqlParms.Add(SQLParam.GetParam("@note_text", _note_text));
                _stageProvider.ExecuteNonQuery("SYNC_Process_Resident_NearestIntersection", _sqlParms);
                this.EntityMessage(string.Format("Processed resident_epec direction for note_entity {0}", _note_entity.ToString()),"INFO");
            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Unable to process resident_epec direction for note_entity: {0}", _note_entity.ToString()),"ERROR");

            }
        }

        private void Delete_Resident_Directions()
        {
            try
            {
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@entity_ref", _script_entity_ref));
                _stageProvider.ExecuteNonQuery("SYNC_Delete_Resident_Directions", _sqlParms);
                this.EntityMessage(string.Format("Deleted resident direction for resident_def {0}",_script_entity_ref ),"INFO");
            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Unable to delete resident direction for resident_def: {0}. ERROR: {1}", _script_entity_ref.ToString(),ex.Message),"ERROR");

            }
        }
        
        private bool Map_Note_Fields()
        {
            bool isMapped = true;

            try
            {
                _notes_def = _dataToMapp["notes_def"].Parse<int>();
                _entity_type = _dataToMapp["entity_type"].Parse<int>();
                _note_entity = _dataToMapp["note_entity"].Parse<int>();
                _note_title = _dataToMapp["title"].ToString();
                _note_text = _dataToMapp["text"].ToString();
            }
            catch { }

            return isMapped;
        }

        private void Parse_Entity_Table(string entityTable)
        {
            string[] temp = entityTable.Split(';');

            _script_entity_type = temp[0].Parse<int>();
            _script_entity_ref = temp[1].Parse<int>();
            _script_title = temp[2];
        }
        #endregion

        #region " Public Methods "
        public void ProcessDataFlow()
        {
            bool processNotes = this.Map_Note_Fields();

            if (processNotes)
            {
                #region " Note logic "
                //Notes have a conditional split
                //Resident_Direction: entity_type == 5 && title == "Directions To Home"

                //Contact Notes: entity_type == 8 && FINDSTRING(title,"Notify",1) == 1

                //Doctor: entity_type == 5 && title == "Doctor"   

                //Hospital: entity_type == 5 && title == "Hospital"

                //Epec Direction: entity_type == 2 && title == "Directions To Home"

                //Responder Notes: entity_type == 8 && title == "Responder Notes"
                #endregion
                switch (_entity_type)
                {
                    case 2:
                        if (_note_title == "Directions To Home")
                            this.Process_Resident_Epec_Directions();
                        break;

                    case 5:
                        if (_note_title == "Directions To Home")
                            this.Process_Resident_Epec_Directions();
                        else if (_note_title == "Doctor")
                            this.Process_Doctor_Note();
                        else if (_note_title == "Hospital")
                            this.Process_Hospital_Note();
                        break;

                    case 8:
                        if (_note_title.Contains("Notify"))
                            this.Process_Responder_Contact_Note();
                        else if (_note_title == "Responder Notes")
                            this.Process_Responder_Contact_Note();
                        break;
                }
            }

        }

        public override void ProcessDataDelete()
        {
            bool isMapped = true;
           // mapped the fields
            try
            {
                _entity_type = this._dataToMapp["entity_type"].Parse<int>();
                _entity_ref = this._dataToMapp["entity_ref"].Parse<int>();
                _update_def = this._dataToMapp["update_def"].Parse<int>();
                this.Parse_Entity_Table(this._dataToMapp["entity_table"].ToString());               
            }
            catch (Exception ex)
            {
                isMapped = false;
            }

            if (isMapped)
            {
                switch (_entity_type)
                {
                    case 5:
                        if (_note_title == "Directions To Home")
                            this.Delete_Resident_Directions();
                        else if (_note_title == "Hospital")
                            this.Delete_Hospital_Note();
                        break;

                    case 8:
                        if ((_note_title.Contains("Notify")) || (_note_title == "Responder Notes"))
                            this._Process_Delete_Resonder_Contact_Note();
                        break;
                }
            }
        }
        #endregion
    }
}
