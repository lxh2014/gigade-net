editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/EdmNew/SaveEdmGroupNewAdd',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'textfield',
                fieldLabel: '編號',
                id: 'group_id',
                name: 'group_id',
                hidden:true,
            }, {
                xtype: 'textfield',
                fieldLabel: '群組名稱',
                id: 'group_name',
                name: 'group_name',
                allowBlank: false
            }, {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                {
                    fieldLabel: '會員電子報',
                    xtype: 'radiogroup',
                    id: 'is_member_edm',
                    labelWidth:100,
                    width: 260,
                    defaults: {
                        name: 'ignore_stockVal'
                    },
                    columns: 2,
                    items: [
                        { id: 'id1', boxLabel: "是", inputValue: '1', checked: true},
                    //{ id: 'id1', boxLabel: "是", inputValue: '1' },
                    { id: 'id2', boxLabel: "否", inputValue: '0' }
                    ]
                }
                ]
            }, {
                xtype: 'textfield',
                fieldLabel: '試閱連接',
                id: 'trial_url',
                name: 'trial_url',
                allowBlank: false,
                submitValue: true,
                vtype: 'url',
            }, {
                xtype: 'numberfield',
                fieldLabel: '排序',
                id: 'sort_order',
                name: 'sort_order',
                minValue: 0,
                allowBlank: false
            }, {
                xtype: 'textareafield',
                fieldLabel: '電子報類型描述',
                id: 'description',
                name: 'description',
                allowBlank: false
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
                        form.submit({
                            params: {
                                group_id: Ext.htmlEncode(Ext.getCmp('group_id').getValue()),
                                group_name: Ext.htmlEncode(Ext.getCmp('group_name').getValue()),
                                is_member_edm: Ext.htmlEncode(Ext.getCmp('is_member_edm').getValue().ignore_stockVal),
                                trial_url: Ext.htmlEncode(Ext.getCmp('trial_url').getValue()),
                                sort_order: Ext.htmlEncode(Ext.getCmp('sort_order').getValue()),
                                description:Ext.htmlEncode(Ext.getCmp('description').getValue()),
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
        title: "新增電子報類型",
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
                    if (row.data.is_member_edm_string.trim().length==0) {
                        Ext.getCmp("id1").setValue(false);
                        Ext.getCmp("id2").setValue(true);
                    } else
                    {
                        Ext.getCmp("id1").setValue(true);
                        Ext.getCmp("id2").setValue(false);
                    }
                    editFrm.getForm().loadRecord(row);
                   
                }
                else {
                    
                    editFrm.getForm().reset();
                   
                }
            }
        }
    });
    editWin.show();
}