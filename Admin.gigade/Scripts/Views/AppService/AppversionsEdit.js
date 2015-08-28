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
Ext.apply(Ext.form.field.VTypes, {
    regxVersions_id: function (val, field) {
        return Versions_idRegstr.test(val)
    },
    regxVersions_idText: REGXVERSIONS_ID
});
/****************  驗證（Vtype）結束  *******************/


/*增加或修改開始*/
//row為null為新增  為1為修改
function SaveReport(row) {
    //平臺數據下拉列表框
    var EditDriver = Ext.create('Ext.form.ComboBox', {
        fieldLabel: DRIVERTEXT,
        store: driverModel,
        id: "cmbdriverEdit",
        queryMode: 'local',
        displayField: 'drivername',
        valueField: 'drivervalue',
        triggerAction: 'all',
        queryMode: 'local',
        selectOnFocus: true,
        forceSelection: true,
        editable: false,
        emptyText: SHULDCHECK + DRIVERTEXT,
        blankText: SHULDCHECK + DRIVERTEXT,
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
            id: 'txtid',
            name: 'txtid',
            submitValue: true,
            hidden: true,
            width: 300
        }, {
            xtype: 'textfield',
            fieldLabel:VERSIONS_ID,
            name: 'txtversions_id',
            id: 'txtversions_id',
            emptyText: SHULDWRITE + VERSIONS_ID,
            allowBlank: false,
            labelWidth: 90,
            width: 300,
            maxLength: '10',
            enforceMaxLength: true,
            vtype: 'regxVersions_id'
        }, {
            xtype: 'textfield',
            fieldLabel: VERSIONS_CODE,
            name: 'txtversions_code',
            id: 'txtversions_code',
            emptyText: SHULDWRITE + VERSIONS_CODE,
            allowBlank: false,
            labelWidth: 90,
            width: 300,
            maxLength: '10',
            enforceMaxLength: true,
            vtype: 'regxVersions_id'
        }, {
            xtype: 'textfield',
            fieldLabel:  VERSIONS_NAME,
            name: 'txtversions_name',
            id: 'txtversions_name',
            allowBlank: false,
            emptyText: SHULDWRITE + VERSIONS_NAME,
            labelWidth: 90,
            width: 300,
            maxLength: '200',
            enforceMaxLength: true
        }, {
            xtype: 'textareafield',
            fieldLabel: VERSIONS_DESC,
            name: 'txtversions_desc',
            id: 'txtversions_desc',
            allowBlank: false,
            emptyText: SHULDWRITE + VERSIONS_DESC,
            labelWidth: 90,
            width: 300,
            maxLength: '255',
            enforceMaxLength: true
        },
        EditDriver, {
            xtype: 'datefield',
            fieldLabel: RELEASE_DATE,
            name: 'daterelease_date',
            id: 'daterelease_date',
            format: 'Y-m-d',
            emptyText: RELEASE_DATE,
            allowBlank: false,
            editable: false,
            labelWidth: 90,
            width: 300
        }
        ],
        buttonAlign: 'center',
        buttons: [{
            text: SAVETEXT,
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                //驗證表單
                if (form.isValid()) {
                    //提交表單
                    form.submit({
                        params: {
                            isAddOrEidt: row,
                            txtid: Ext.getCmp("txtid").getValue(),
                            txtversions_id: Ext.getCmp("txtversions_id").getValue(),
                            txtversions_name: Ext.getCmp("txtversions_name").getValue(),
                            txtversions_desc: Ext.getCmp("txtversions_desc").getValue(),
                            txtversions_code: Ext.getCmp("txtversions_code").getValue(),
                            cmbdriverEdit: Ext.getCmp("cmbdriverEdit").getValue(),
                            daterelease_date: Ext.getCmp("daterelease_date").getValue()
                        },
                        success: function (from, action) {
                            var result = action.result.msg;
                            Ext.MessageBox.alert(MESSAGEINFO, result,
                                            function () {
                                                ReportWin.close();
                                                AppVersionsStore.load();
                                            });
                        },
                        failure: function (response) {
                            var resText = eval("(" + response.responseText + ")");
                            Ext.Msg.alert(resText.msg);
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
        }]
    }).show();
}
/*增加或修改結束*/


