/*
* 文件名稱 :AppversionsEdit.js
* 文件功能描述 :上架版本編輯JS
* 版權宣告 :
* 開發人員 : 肖國棟
* 版本資訊 : 1.0
* 日期 : 2015.8.25
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/


/****************  驗證（Vtype）開始  *******************/
//版本ID驗證
var Versions_idRegstr = /^\d*$/;
/****************  驗證（Vtype）結束  *******************/


/*增加或修改開始*/
//row為null為新增  為1為修改
function SaveReport(row,store) {
    //平臺數據下拉列表框
    var EditDriver = Ext.create('Ext.form.ComboBox', {
        fieldLabel: DRIVERTEXT,
        store: driverModel,
        id: "drive",
        name:"drive",
        queryMode: 'local',
        displayField: 'drivername',
        valueField: 'drivervalue',
        triggerAction: 'all',
        queryMode: 'local',
        selectOnFocus: true,
        allowBlank: false,
        forceSelection: true,
        editable: false,
        emptyText: SHULDWRITEDRIVERTEXT,
        blankText: SHULDWRITEDRIVERTEXT,
        labelWidth: 90,
        width: 300
    });
    //創建 保存數據 的 form 表單
    var ReportForm = new Ext.form.Panel({
        frame: true,
        plain: true,
        border: false,
        bodyStyle: "padding: 12px 12px 6px 12px;",
        url: '/AppService/EditAppversionsInfo',
        items: [{
            //定義 隱藏的 textfield 以便獲取 rid 的值
            xtype: 'textfield',
            fieldLabel: RID,
            id: 'id',
            name: 'id',
            submitValue: true,
            hidden: true,
            width: 300
        }, {
            xtype: 'textfield',
            fieldLabel:VERSIONS_ID,
            name: 'versions_id',
            id: 'versions_id',
            emptyText: SHULDWRITEVERSIONS_ID,
            allowBlank: false,
            labelWidth: 90,
            width: 300,
            maxLength: '10',
            enforceMaxLength: true,
            regex: Versions_idRegstr,
            regexText:RGTXTINT
        }, {
            xtype: 'textfield',
            fieldLabel: VERSIONS_CODE,
            name: 'versions_code',
            id: 'versions_code',
            emptyText: SHULDWRITEVERSIONS_CODE,
            allowBlank: false,
            labelWidth: 90,
            width: 300,
            maxLength: '10',
            enforceMaxLength: true,
            regex: Versions_idRegstr,
            regexText: RGTXTINT
        }, {
            xtype: 'textfield',
            fieldLabel:  VERSIONS_NAME,
            name: 'versions_name',
            id: 'versions_name',
            allowBlank: false,
            emptyText: SHULDWRITEVERSIONS_NAME,
            labelWidth: 90,
            width: 300,
            maxLength: '200',
            enforceMaxLength: true
        }, {
            xtype: 'textareafield',
            fieldLabel: VERSIONS_DESC,
            name: 'versions_desc',
            id: 'versions_desc',
            allowBlank: false,
            emptyText: SHULDWRITEVERSIONS_DESC,
            labelWidth: 90,
            width: 300,
            maxLength: '255',
            enforceMaxLength: true
        },
        EditDriver, {
            xtype: 'datefield',
            fieldLabel: RELEASE_DATE,
            name: 'releasedateQuery',
            id: 'releasedateQuery',
            format: 'Y-m-d',
            editable: false,
            labelWidth: 90,
            width: 300
        },
        {
            xtype: 'checkboxfield',
            fieldLabel: YESORNORELEASE,
            name: 'release_type',
            id: 'release_type',
            boxLabel: RELEASE,
            checked:true
        }
        ],
        buttonAlign: 'center',
        buttons: [{
            text: SAVETEXT,
            id: 'btnSave',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                //驗證表單
                if (form.isValid()) {
                    Ext.getCmp('btnSave').setDisabled(true);
                    //提交表單
                    form.submit({
                        params: {
                            txtid: Ext.getCmp("id").getValue(),
                            txtversions_id: Ext.getCmp("versions_id").getValue(),
                            txtversions_name: Ext.getCmp("versions_name").getValue(),
                            txtversions_desc: Ext.getCmp("versions_desc").getValue(),
                            txtversions_code: Ext.getCmp("versions_code").getValue(),
                            cmbdriverEdit: Ext.getCmp("drive").getValue(),//選擇的裝置
                            daterelease_date: Ext.getCmp("releasedateQuery").getValue(),
                            status_type: Ext.getCmp("release_type").getValue()==true?1:0
                        },
                        success: function (from, action) {
                            var result = action.result.msg;
                            Ext.MessageBox.alert(MESSAGEINFO, result,
                                            function () {
                                                AppVersionsStore.load();
                                                ReportWin.close();
                                            });
                        },
                        failure: function (response) {
                            var resText = eval("(" + response.responseText + ")");
                            Ext.Msg.alert(MESSAGEINFO, resText.msg);
                            Ext.getCmp('btnSave').setDisabled(false);
                        }
                    })
                }
            }
        }]
    });
    //創建 保存數據 的 window 窗體
    var ReportWin = new Ext.Window({
        title: row == null ? ADDINFOMENU : UPDATEINFOMENU,
        width: 360,
        height: 350,
        iconCls: row == null ? 'ui-icon ui-icon-add' : 'ui-icon ui-icon-pencil',
        plain: true,
        border: false,
        modal: true,
        resizable: false,
        draggable: true,
        bodyStyle: "padding: 10px 10px 7px 10px;",
        layout: 'fit',
        items: [ReportForm],
        closable: false,
        tools: [{
            type: 'close',
            handler: function (event, toolEl, panel) {
                ReportWin.destroy();
            }
        }],
        listeners: {
            'show': function () {
                if (row) {
                    ReportForm.getForm().loadRecord(row); //如果是編輯的話
                }
                else {
                    ReportForm.getForm().reset(); //如果是編輯的話
                }
            }
        }
    }).show();
}
/*增加或修改結束*/
