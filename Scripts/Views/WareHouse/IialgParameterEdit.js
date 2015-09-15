//庫調參數設定新增頁面
var Rowid;
Ext.define("gigade.paraModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Rowid', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});
//參數類型store
var ParameterStore = Ext.create('Ext.data.Store', {
    model: 'gigade.paraModel',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/WareHouse/GetParameter",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});
ParameterStore.on('beforeload', function () {
    Ext.apply(ParameterStore.proxy.extraParams, {
        pn: 1
    });
});

function addFunction(row, store) {
    
    var addFrm = Ext.create('Ext.form.Panel', {
        id: 'addFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        labelWidth: 120,
        url: '/WareHouse/InsertTP',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'combobox',
                fieldLabel: '參數類型',
                id: 'ParameterType1',
                name: 'ParameterType1',
                store: ParameterStore,
                displayField: 'parameterName',
                valueField: 'Rowid',
                //queryMode: 'local',
                hiddenName: 'ParameterType',
                //typeAhead: true,
                //forceSelection: false,
                //emptyText: 'SELECT',
                allowBlank: false,
                listeners: {
                    'blur': function (a, b) {
                        Ext.Ajax.request({
                            url: "/WareHouse/GetParameter",
                            method: 'post',
                            type: 'text',
                            params: {
                                pn: 2,
                                parameterName: Ext.getCmp('ParameterType1').getRawValue()
                            },
                            success: function (form, action) {
                                var result = Ext.decode(form.responseText);
                                if (result.success) {
                                    if (result.data.length > 0) {
                                        Rowid = result.data[0].Rowid;
                                        Ext.getCmp("parameter_Name").show();
                                        Ext.getCmp("ParameterType2").show();
                                        Ext.getCmp("parameter_Name").allowBlank = false;
                                        Ext.getCmp("ParameterType2").allowBlank = false;
                                    }
                                    else {
                                        Ext.getCmp("ParameterType2").hide();
                                        Ext.getCmp("parameter_Name").hide();
                                        Ext.getCmp("ParameterType2").allowBlank = true;
                                        Ext.getCmp("parameter_Name").allowBlank = true;
                                    }
                                }
                            }
                        });
                    }
                }
            },           
            {
                xtype: 'textfield',
                fieldLabel: "參數類型代碼",
                name: 'ParameterType2',
                id: 'ParameterType2',
                hidden: true,
                allowBlank: false
            },
            {
                xtype: 'textfield',
                fieldLabel: "參數名稱",
                name: 'parameter_Name',
                id: 'parameter_Name',
                hidden: true,
                allowBlank: false
            },
            {
                xtype: 'textfield',
                fieldLabel: "Code代碼",
                name: 'parameter_Code',
                id: 'parameter_Code',
                allowBlank: false
            },
            {
                xtype: 'textfield',
                fieldLabel: "備註",
                name: 'remark1',
                id: 'remark1',
                allowBlank: false
            }
        ],
        buttons: [
        {
            text: SAVE,
            formBind: true,
            handler: function () {
                var form = this.up('form').getForm();

                if (form.isValid()) {
                    form.submit({
                        params: {
                            ParameterType: Ext.htmlEncode(Ext.getCmp('ParameterType1').getValue()),//參數類型
                            Parameter_type: Ext.htmlEncode(Ext.getCmp('ParameterType2').getValue()),//參數類型代碼
                            parameterName: Ext.htmlEncode(Ext.getCmp('parameter_Name').getValue()),//參數名稱
                            parameterCode: Ext.htmlEncode(Ext.getCmp('parameter_Code').getValue()),
                            remark: Ext.htmlEncode(Ext.getCmp('remark1').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);                            
                            if (result.success) {
                                if (result.msg == 0)
                                {
                                    Ext.Msg.alert(INFORMATION, "新增成功!");
                                    TPStore.load();
                                    addWin.close();
                                } else if (result.msg == 1) {
                                    Ext.Msg.alert(INFORMATION, "新增失敗!");
                                } else if (result.msg == 2) {
                                    Ext.Msg.alert(INFORMATION, "code重複!");
                                }
                            } else {
                                Ext.Msg.alert(INFORMATION, "新增失敗!");
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, "出現異常");
                        }
                    });
                }
            }
        }]
    });
    var addWin = Ext.create('Ext.window.Window', {
        title: "參數新增",
        id: 'addWin',
        iconCls: 'icon-user-edit',
        width: 550,
        height: 380,
        autoScroll: true,
        //        height: document.documentElement.clientHeight * 260 / 783,
        layout: 'fit',
        items: [addFrm],
        closeAction: 'destroy',
        modal: true,
        constrain: true,    //窗體束縛在父窗口中
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: CLOSEFORM,
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('addWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }]
        ,
        listeners: {
            'show': function () {
                if (row == null) {
                    // addFrm.getForm().loadRecord(row); //如果是添加的話
                    addFrm.getForm().reset();
                } else {
                    addFrm.getForm().loadRecord(row); //如果是編輯的

                }
            }
        }
    });
    addWin.show();
}

function UpdFunction(row, store)
{
    if (row != null) {
        Rowid = row.data["Rowid"];
    }
    Rowid;
    var UpdFrm = Ext.create('Ext.form.Panel', {
        id: 'UpdFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        labelWidth: 120,
        url: '/WareHouse/UpdTP',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'textfield',
                fieldLabel: "參數名稱",
                name: 'parameterName',
                id: 'parameterName',
                allowBlank: false
            },
            {
                xtype: 'textfield',
                fieldLabel: "參數類型代碼",
                name: 'ParameterType',
                id: 'ParameterType',
                allowBlank: false
            },
            {
                xtype: 'textfield',
                fieldLabel: "Code代碼",
                name: 'ParameterCode',
                id: 'ParameterCode',
                allowBlank: false
            }
            ,
            {
                xtype: 'textfield',
                fieldLabel: "備註",
                name: 'remark',
                id: 'remark',
                allowBlank: false
            }
        ],
        buttons: [
        {
            text: SAVE,
            formBind: true,
            handler: function () {
                var form = this.up('form').getForm();  
                if (form.isValid()) {
                    form.submit({
                        params: {
                            rowid:Rowid,
                            parameterName: Ext.htmlEncode(Ext.getCmp('parameterName').getValue()),//參數名稱
                            ParameterCode: Ext.htmlEncode(Ext.getCmp('ParameterCode').getValue()),
                            ParameterType2: Ext.htmlEncode(Ext.getCmp('ParameterType').getValue()),
                            remark: Ext.htmlEncode(Ext.getCmp('remark'))
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);                            
                            if (result.success) {
                                if (result.msg == 0) {
                                    Ext.Msg.alert(INFORMATION, "更新成功");
                                    TPStore.load();
                                    UpdWin.close();
                                } else if (result.msg == 1)
                                {
                                    Ext.Msg.alert(INFORMATION, "更新失敗");
                                } 
                            } else {
                                Ext.Msg.alert(INFORMATION, "更新失敗");
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, "出現異常");
                        }
                    });
                }
            }
        }]
    });
    var UpdWin = Ext.create('Ext.window.Window', {
        title: "參數新增",
        id: 'UpdWin',
        iconCls: 'icon-user-edit',
        width: 550,
        height: 380,
        autoScroll: true,
        //        height: document.documentElement.clientHeight * 260 / 783,
        layout: 'fit',
        items: [UpdFrm],
        closeAction: 'destroy',
        modal: true,
        constrain: true,    //窗體束縛在父窗口中
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: CLOSEFORM,
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('UpdWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }]
        ,
        listeners: {
            'show': function () {
                if (row == null) {
                    // UpdFrm.getForm().loadRecord(row); //如果是添加的話
                    UpdFrm.getForm().reset();
                } else {
                    UpdFrm.getForm().loadRecord(row); //如果是編輯的

}
            }
        }
    });
    UpdWin.show();
}