/*
* 文件名稱 :AppNotifyPoolEdit.js
* 文件功能描述 :推播設定編輯JS
* 版權宣告 :
* 開發人員 : 肖國棟
* 版本資訊 : 1.0
* 日期 : 2015.8.24
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/



/****************  驗證（Vtype）開始  *******************/
//時間驗證
Ext.apply(Ext.form.field.VTypes, {
    regxvalid_end: function (val, field) {
        var startDate = Ext.getCmp('datevalid_start').getValue();
        var endDate = Ext.getCmp('datevalid_end').getValue();
        if (endDate) {
            if (startDate > endDate) {
                return false;
            }
            else {
                return true;
            }
        }
    },
    regxvalid_endText: REGXVALID_END
});
/****************  驗證（Vtype）結束  *******************/

/*增加或修改開始*/
//row為null為新增  為1為修改
function SaveReport(row) {
    //創建 保存數據 的 form 表單
    var ReportForm = new Ext.form.Panel({
        frame: true,
        plain: true,
        border: false,
        bodyStyle: "padding: 12px 12px 6px 12px;",
        url: '/AppService/EditAppNotifyPoolInfo',
        items: [{
            //定義 隱藏的 textfield 以便獲取 rid 的值
            xtype: 'textfield',
            fieldLabel: RID,
            id: 'txtid',
            name: 'txtid',
            hidden: true,
            width: 300
        }, {
            xtype: 'textfield',
            fieldLabel: TITLE,
            name: 'txttitle',
            id: 'txttitle',
            emptyText: SHULDWRITETITLE,
            allowBlank: false,
            submitValue: true,
            labelWidth: 90,
            width: 300,
            maxLength: '500',
            enforceMaxLength: true
        }, {
            xtype: 'textareafield',
            fieldLabel: ALERTTXT,
            name: 'txtafalert',
            id: 'txtafalert',
            emptyText: SHULDWRITEALERTTXT,
            allowBlank: false,
            submitValue: true,
            labelWidth: 90,
            width: 300,
            maxLength: '1000',
            enforceMaxLength: true
        }, {
            xtype: 'textfield',
            fieldLabel: URLTEXT,
            name: 'txturl',
            id: 'txturl',
            labelWidth: 90,
            submitValue: true,
            width: 300,
            maxLength: '1000',
            enforceMaxLength: true,
            vtype: 'url'
        },
        {
            xtype: 'textfield',
            fieldLabel: TOTEXT,
            name: 'txtto',
            id: 'txtto',
            allowBlank: false,
            submitValue: true,
            emptyText: SHULDWRITETOTEXT,
            labelWidth: 90,
            width: 300
        }, {
            xtype: 'datefield',
            fieldLabel: VALID_START,
            name: 'datevalid_start',
            id: 'datevalid_start',
            format: 'Y-m-d',
            emptyText: SHULDWRITEVALID_START,
            allowBlank: false,
            editable: false,
            submitValue: true,
            labelWidth: 90,
            width: 300
        }, {
            xtype: 'datefield',
            fieldLabel: VALID_END,
            name: 'datevalid_end',
            id: 'datevalid_end',
            format: 'Y-m-d',
            emptyText: SHULDWRITEVALID_END,
            allowBlank: false,
            submitValue: true,
            editable: false,
            labelWidth: 90,
            width: 300,
            vtype: 'regxvalid_end'
        }, {
            xtype: 'radiogroup',
            id: 'now_state',
            name: 'now_state',
            fieldLabel: NOW_STATE,
            labelWidth: 90,
            allowBlank: false,
            submitValue: true,
            disabled: row != null ? false : true,
            hidden: true,
            defaults: {
                name: 'state'
            },
            layout: 'column',
            vertical: true,
            items: [{ boxLabel: BOXFOU, inputValue: '0', checked: true },
                { boxLabel: BOXSHI, inputValue: '1' }]
        }],
        buttonAlign: 'center',
        buttons: [{
            formBind: true,
            disabled: true,
            id: 'btnSave',
            text: SAVETEXT,
            handler: function () {
                var form = this.up('form').getForm();
                //驗證表單
                if (form.isValid()) {
                    Ext.getCmp('btnSave').setDisabled(true);
                    //提交表單
                    form.submit({
                        params: {
                            isAddOrEidt: row,
                            txtid: Ext.getCmp("txtid").getValue(),
                            txttitle: Ext.getCmp("txttitle").getValue(),
                            txtafalert: Ext.getCmp("txtafalert").getValue(),
                            txturl: Ext.getCmp("txturl").getValue(),
                            txtto: Ext.getCmp("txtto").getValue(),
                            datevalid_start: Ext.getCmp("datevalid_start").getValue(),
                            datevalid_end: Ext.getCmp("datevalid_end").getValue(),
                            now_state: Ext.getCmp("now_state").getValue()
                        },
                        success: function (from, action) {
                            var result = action.result.msg;
                            Ext.MessageBox.alert(MESSAGEINFO, result,
                                            function () {
                                                ReportWin.close();
                                            });
                        },
                        failure: function (response) {
                            var resText = eval("(" + response.responseText + ")");
                            Ext.Msg.alert(resText.msg);
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
        }]
    }).show();
}
/*增加或修改結束*/