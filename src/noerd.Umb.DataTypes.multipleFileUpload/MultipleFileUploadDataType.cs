using System;
using System.Collections;
using umbraco.cms.businesslogic.datatype;
using umbraco.editorControls;
using umbraco.interfaces;
using BaseDataType=umbraco.cms.businesslogic.datatype.BaseDataType;
using DefaultData=umbraco.cms.businesslogic.datatype.DefaultData;

namespace noerd.Umb.DataTypes.multipleFileUpload
{
    /// <summary>
    /// Umbraco IDataType: Multiple file upload
    /// </summary>
    public class MultipleFileUploadDataType : BaseDataType, IDataType
    {
        // -------------------------------------------------------------------------
        // Fields
        // -------------------------------------------------------------------------

        private IDataPrevalue _prevalueEditor;
        private IDataEditor _editor;
        private IData _baseData;

        // -------------------------------------------------------------------------
        // Public members
        // -------------------------------------------------------------------------

        // IDataType Members
        // -------------------------------------------------------------------------

        #region IDataType Members

        public override Guid Id
        {
            get { return new Guid("ACCB9911-CD81-4B17-AF3F-446CEB1DBF0D"); }
        }

        // -------------------------------------------------------------------------

        public override string DataTypeName
        {
            get { return "Multiple File Upload"; }
        }

        // -------------------------------------------------------------------------

        public override IDataEditor DataEditor
        {
            get
            {
                if (_editor == null)
                {
                    DataTypeDefinition dataTypeDef = DataTypeDefinition.GetByDataTypeId(Id);
                    SortedList preValues = PreValues.GetPreValues(dataTypeDef.Id);

                    string prevalue = "";
                    if (preValues.Count > 0)
                        prevalue = ((PreValue) preValues[0]).Value;
                    
                    _editor = new MultipleFileUpload(Data, prevalue);
                }
                return _editor;
            }
        }

        // -------------------------------------------------------------------------

        public override IDataPrevalue PrevalueEditor
        {
            get
            {
                if (_prevalueEditor == null)
                    _prevalueEditor = new DefaultPrevalueEditor(this, true);
                return _prevalueEditor;
            }
        }

        // -------------------------------------------------------------------------

        public override IData Data
        {
            get
            {
                if (_baseData == null)
                    _baseData = new DefaultData(this);
                return _baseData;
            }
        }

        #endregion
    }
}
