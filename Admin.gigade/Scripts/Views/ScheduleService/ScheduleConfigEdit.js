/*************************************************************************************添加 編輯 框*************************************************************************************************/
//參數碼列表
var ParameterCodeStore = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' },
    ],
    autoLoad: true,
    proxy: {
        type: 'ajax',//GetParameterCodeList
        url: "/Parameter/QueryPara?paraType=schedule_config",
        noCache: false,
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
//ParameterCodeStore.on("beforeload", function ()
//{
//    Ext.apply(ParameterCodeStore.proxy.extraParams, {
//        paraType: 'schedule_config',
//    })
//})
editFunction_config = function (row, store)
{
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/ScheduleService/SaveScheduleConfigInfo',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: 'rowid',
                id: 'rowid',
                name: 'rowid',
                hidden: true
            },
            {
                xtype: 'combobox',
                editable: false,
                fieldLabel: '排程Code',
                id: 'schedule_code_config',
                name: 'schedule_code_config',
                allowBlank: false,
                displayField: 'schedule_code',
                valueField: 'schedule_code',
                store: Schedule_Code_Store,
                value: Ext.getCmp("schedule_code").getValue(),
                
            },
            {
                xtype: 'combobox',
                editable: false,
                fieldLabel: '參數名稱',
                id: 'parameterCode',
                name: 'parameterCode',
                allowBlank: false,
                displayField: 'parameterName',
                valueField: 'parameterCode',
                store: ParameterCodeStore,
                //value: Ext.getCmp("schedule_code").getValue(),
                //listeners: {
                //    change: function ()
                //    {
                //        Ext.getCmp("parameterName").setValue(Ext.getCmp('parameterCode').getRawValue());
                //    }
                //}
            },
            {
                xtype: 'textfield',
                fieldLabel: '參數值',
                id: 'value',
                name: 'value',
                allowBlank: false,
            },
            
        ],

        // 点击保存按钮后  提示信息 
        buttons: [
            {
                formBind: true,
                disabled: true,
                text: '保存',
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        form.submit({
                            params: {
                                rowid: Ext.htmlEncode(Ext.getCmp('rowid').getValue()),
                                schedule_code: Ext.htmlEncode(Ext.getCmp('schedule_code_config').getValue()),
                                parameterCode: Ext.htmlEncode(Ext.getCmp('parameterCode').getValue()),
                                value: Ext.htmlEncode(Ext.getCmp('value').getValue()),
                                parameterName: Ext.htmlEncode(Ext.getCmp('parameterCode').getRawValue()),
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    Ext.Msg.alert(INFORMATION, "保存成功! ");
                                   // store.load();
                                     Schedule_Config_Store.load();
                                    editWin.close();
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                    editWin.close();
                                }
                            },
                            failure: function () {
                                Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                editWin.close();
                            }
                        });
                    }
                }
            },
        ]
    });


    //点击关闭按钮后  提示信息
    //一个指定的打算作为一个应用程序窗口的面板。
    var editWin = Ext.create('Ext.window.Window', {
        title: "新增信息",
        id: 'editWin',
        iconCls: "icon-user-add",
        width: 460,
        height: 260,
        layout: 'fit',//布局样式
        items: [editFrm],
        constrain: true, //束縛窗口在框架內
        closeAction: 'destroy',
        modal: true,
        resizable: false,//false 禁止調整windows窗體的大小
        // reaizable:true,// true  允許調整windows窗體的大小
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: "關閉窗口",
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm("提示信息", "是否關閉窗口", function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }],
        listeners: {
            'show': function () {
                if (row) {
                    editFrm.getForm().loadRecord(row);
                    Ext.getCmp('schedule_code_config').setValue(row.data.schedule_code);
                    //initRow(row);
                }
                else {
                    editFrm.getForm().reset();
                }
            }
        }
    });

    editWin.show();

}