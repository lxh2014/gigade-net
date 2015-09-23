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
            }, {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                {
                    fieldLabel: '會員電子報',
                    xtype: 'radiogroup',
                    id: 'is_member_edm',
                    labelWidth:95,
                    width: 260,
                    defaults: {
                        name: 'ignore_stockVal'
                    },
                    columns: 2,
                    items: [
                    { id: 'id1', boxLabel: "是", inputValue: '1', checked: true },
                    { id: 'id2', boxLabel: "否", inputValue: '0' }
                    ]
                }
                ]
            }, {
                xtype: 'numberfield',
                fieldLabel: '排序',
                id: 'sort_order',
                name: 'sort_order',
                minValue:0,
            }, {
                xtype: 'textareafield',
                fieldLabel: '電子報類型描述',
                id: 'description',
                name: 'description',
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
        title: "新增電子報類別",
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
                    editFrm.getForm().reset();
                }
            }
        }
    });
    editWin.show();
}