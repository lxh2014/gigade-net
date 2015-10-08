editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/EdmNew/SaveEdmTemplateAdd',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
             {
                 xtype: 'displayfield',
                 fieldLabel: '編號',
                 id: 'template_id',
                 name: 'template_id',
                 
             }, {
                 xtype: 'textfield',
                 fieldLabel: '範本名稱',
                 id: 'template_name',
                 name: 'template_name',
                 allowBlank: false
             }, {
                 xtype: 'textfield',
                 fieldLabel: '內容編輯網址',
                 id: 'edit_url',
                 name: 'edit_url',
                 allowBlank: false,
                 submitValue: true,
                 vtype: 'url',
                 
             }, {
                 xtype: 'textfield',
                 fieldLabel: '內容產生網址',
                 id: 'content_url',
                 name: 'content_url',
                 allowBlank: false,
                 submitValue: true,
                 vtype: 'url',
             }, {
                 xtype: 'displayfield',
                 fieldLabel: '建立日期',
                 id: 'template_createdate',
                 name: 'template_createdate',
                 
             }, {
                 xtype: 'displayfield',
                 fieldLabel: '更新日期',
                 id: 'template_updatedate',
                 name: 'template_updatedate',
                 
             }, {
                 xtype: 'displayfield',
                 fieldLabel: '建立者',
                 id: 'template_create_user',
                 name: 'template_create_user',
                
             }, {
                 xtype: 'displayfield',
                 fieldLabel: '修改者',
                 id: 'template_update_user',
                 name: 'template_update_user',
             },
        ],
        buttons: [
            {
                formBind: true,
                disabled: true,
                text: '保存',
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        this.disable();
                        form.submit({
                            params: {
                                template_id: Ext.htmlEncode(Ext.getCmp('template_id').getValue()),
                                template_name: Ext.htmlEncode(Ext.getCmp('template_name').getValue()),
                                edit_url: Ext.htmlEncode(Ext.getCmp('edit_url').getValue().ignore_stockVal),
                                content_url: Ext.htmlEncode(Ext.getCmp('content_url').getValue()),
                                template_create_user: Ext.htmlEncode(Ext.getCmp('template_create_user').getValue()),
                                template_update_user: Ext.htmlEncode(Ext.getCmp('template_update_user').getValue().ignore_stockVal),
                                template_createdate: Ext.htmlEncode(Ext.getCmp('template_createdate').getValue()),
                                template_updatedate: Ext.htmlEncode(Ext.getCmp('template_updatedate').getValue()),
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    Ext.Msg.alert(INFORMATION, "保存成功! ");
                                    store.load();
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
    var editWin = Ext.create('Ext.window.Window', {
        title: "新增電子報範本",
        id: 'editWin',
        iconCls: "icon-user-add",
        width: 360,
        height: 300,
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
                }
                else {
                    Ext.getCmp('template_id').hide();
                    Ext.getCmp('template_create_user').hide();
                    Ext.getCmp('template_update_user').hide();
                    Ext.getCmp('template_createdate').hide();
                    Ext.getCmp('template_updatedate').hide();
                    editFrm.getForm().reset();
                }
            }
        }
    });
    editWin.show();
}