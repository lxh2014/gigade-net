/*
* 文件名稱 :AppMessageEdit.js
* 文件功能描述 :訊息公告編輯JS
* 版權宣告 :
* 開發人員 : 白明威
* 版本資訊 : 1.0
* 日期 : 2015.8.27
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/
Ext.apply(Ext.form.field.VTypes, {
    regxvalid_end: function (val, field) {
        var startDate = Ext.getCmp('new_msg_start').getValue();
        var endDate = Ext.getCmp('new_msg_end').getValue();
        if (endDate) {
            if (startDate > endDate) {
                return false;
            }
            else {
                return true;
            }
        }
        return true;
    },
    regxvalid_endText: START_TIME_CANT_LT_END_TIME
});

var pcFrm = Ext.create('Ext.form.Panel', {
    id: 'pcFrm',
    layout: 'anchor',
    frame: true,
    plain: true,
    border: false,
    bodyStyle: "padding: 12px 12px 6px 12px;",
    url: '/AppService/AppMessageInsert',
    items: [{
        xtype: 'textfield',
        id: 'new_title',
        fieldLabel: TITLE,//標題
        width: 300,
        allowBlank: false,
        emptyText: SHULDWRITETITLE,
        maxLength: '20',
        enforceMaxLength: true
    }, {
        xtype: 'textfield',
        id: 'new_linkurl',
        fieldLabel: LINKURL,//URL
        width: 300,
        vtype: 'url',
        maxLength: '255',
        enforceMaxLength: true
    }, {
        xtype: 'textarea',
        id: 'new_content',
        fieldLabel: CONTENT,//內容
        margin: '0 0 20 0',
        height: 60,
        width: 300,
        allowBlank: false,
        emptyText: SHULDWRITECONTENT,
        maxLength: '255',
        enforceMaxLength: true
    }, {
        xtype: 'datetimefield',
        format: 'Y-m-d H:i:s',
        id: 'new_msg_start',
        fieldLabel: MSG_START_TIME,//開始時間
        width: 300,
        allowBlank: false,
        editable: false,
        vtype: 'regxvalid_end',
        emptyText: SHULDWRITEMSG_START_TIME,
    }, {
        xtype: 'datetimefield',
        format: 'Y-m-d H:i:s',
        id: 'new_msg_end',
        fieldLabel: MSG_END_TIME,//結束時間
        width: 300,
        allowBlank: false,
        editable: false,
        vtype: 'regxvalid_end',
        emptyText: SHULDWRITEMSG_END_TIME,
    }, {
        xtype: 'combobox',
        id: 'new_fit_os',
        store: FitOsStore,
        fieldLabel: FIT_OS,//適用平台
        displayField: 'parameterName',
        width: 300,
        queryMode: 'local',
        allowBlank: false,
        emptyText: SHULDWRITEFIT_OS,
        maxLength: '20',
        enforceMaxLength: true
    }, {
        xtype: 'combobox',
        id: 'new_display_type',
        store: DisplayTypeStore,
        displayField: 'parameterName',
        valueField: 'parameterCode',
        fieldLabel: DISPLAY_TYPE,//顯示類別
        width: 300,
        editable: false,
        //disabled: true,
        //hidden: true
    }, {
        xtype: 'textfield',
        id: 'new_appellation',
        width: 300,
        fieldLabel: APPELLATION,//稱謂
        allowBlank: false,
        emptyText: SHULDWRITEAPPELLATION,
        maxLength: '100',
        enforceMaxLength: true
    }],
    buttonAlign: 'center',
    buttons: [{
        text: SAVE,//保存
        id: 'btnSave',
        formBind: true,
        disabled: true,
        handler: function () {
            var form = this.up('form').getForm();
            if (Ext.getCmp('new_msg_start').getValue() > Ext.getCmp('new_msg_end').getValue()) {
                Ext.Msg.alert(INFORMATION, START_TIME_CANT_LT_END_TIME);
                return;
            }
            if (form.isValid()) {
                Ext.getCmp('btnSave').setDisabled(true);
                form.submit({
                    params: getParams(),
                    success: function (form, action) {
                        var result = Ext.decode(action.response.responseText);
                        if (result.success) {
                            addPc.hide();
                            pcFrm.getForm().reset();
                            AppMessageStore.load();
                            //FitOsStore.load();
                            Ext.Msg.alert(INFORMATION, INSERT_NEW_SUCCESS);//新增數據成功
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, INSERT_NEW_FILED);//新增數據失敗
                        Ext.getCmp('btnSave').setDisabled(false);
                    }
                });
            }
        }
    }]
})
var addPc = Ext.create('Ext.window.Window', {
    title: INSERT_NEW,//添加記錄
    id: 'addPc',
    width: 360,
    height: 400,
    iconCls: 'ui-icon ui-icon-add',
    plain: true,
    border: false,
    modal: true,
    resizable: false,
    draggable: true,
    hidden: true,
    bodyStyle: "padding: 10px 10px 7px 10px;",
    layout: 'fit',
    items: [pcFrm],
    closable: false,
    tools: [{
        type: 'close',
        handler: function (event, toolEl, panel) {
            addPc.hide();
        }
    }]
})
function getParams() {
    var params = new Object();
    params.title = Ext.getCmp('new_title').getValue();
    params.content = Ext.getCmp('new_content').getValue();
    params.linkurl = Ext.getCmp('new_linkurl').getValue();
    params.msg_start = Ext.getCmp('new_msg_start').getValue();
    params.msg_end = Ext.getCmp('new_msg_end').getValue();
    params.appellation = Ext.getCmp('new_appellation').getValue();
    params.fit_os = Ext.getCmp('new_fit_os').getValue();
    params.display_type = Ext.getCmp('new_display_type').getValue();
    return params;
}