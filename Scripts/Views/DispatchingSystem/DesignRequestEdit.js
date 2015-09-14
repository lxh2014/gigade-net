editFunction = function (row, store) {
    Ext.define('TypeModel', {
        extend: 'Ext.data.Model',
        fields: [
             { name: "parameterCode", type: "int" },
            { name: "parametername", type: "string" }

        ]
    });

    var TypeStore = Ext.create('Ext.data.Store', {
        autoDestroy: true,
        model: 'TypeModel',
        remoteSort: false,
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: "/DispatchingSystem/GetType",
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });


    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/DispatchingSystem/DesignRequestEdit',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 120 },
        items: [
             {
                 xtype: 'displayfield',
                 fieldLabel: 'ID',
                 labelWidth: 110,
                 width: 400,
                 id: 'dr_id',
                 name: 'dr_id',
                 hidden: true
             },
             {
                 xtype: 'displayfield',
                 fieldLabel: '需求提出者',
                 labelWidth: 110,
                 width: 400,
                 id: 'dr_requester_id_name',
                 name: 'dr_requester_id_name',
                 hidden: false
             },
             {
                 xtype: 'combobox',
                 fieldLabel: "需求類型",
                 labelWidth: 110,
                 id: 'dr_type',
                 name: 'dr_type',
                 width: 400,
                 store: TypeStore,
                 emptyText: '請選擇',
                 allowBlank: true,
                 forceSelection: true,
                 typeAhead: true,
                 triggerAction: 'all',
                 displayField: 'parametername',
                 valueField: 'parameterCode',
                 listeners: {
                     'select': function () {
                         var num = Ext.getCmp('dr_type').getValue();
                         switch (num) {
                             case 4:
                             case 5:
                                 Ext.getCmp('product_id').show();
                                 Ext.getCmp('product_name').show();
                                 var product_id = Ext.getCmp('product_id');
                                 product_id.allowBlank = false;
                                 product_id.show();
                                 break;
                             default:
                                 Ext.getCmp('product_name').hide();
                                 var product_id = Ext.getCmp('product_id');
                                 product_id.allowBlank = true;
                                 product_id.hide();
                                 break;
                         }
                     }
                 }
             },
            {
                xtype: 'textfield',
                fieldLabel: "商品編號",
                id: 'product_id',
                name: 'product_id',
                labelWidth: 110,
                width: 400,
                allowBlank: false,
                regex: /^[0-9]+$/,
                regexText: '格式不正確',
                listeners: {
                    blur: function () {
                        var id = Ext.getCmp('product_id').getValue();
                        Ext.Ajax.request({
                            url: "/DispatchingSystem/GetProductName",
                            method: 'post',
                            type: 'text',
                            params: {
                                id: Ext.getCmp('product_id').getValue()
                            },
                            success: function (form, action) {
                                var result = Ext.decode(form.responseText);
                                if (result.success) {
                                    var msg = result.msg;
                                    if (msg == 0) {
                                        Ext.getCmp("product_name").setValue("沒有該商品信息！");
                                        Ext.getCmp("product_id").setValue("");
                                    }
                                    else {
                                        Ext.getCmp("product_name").setValue(msg);
                                    }
                                }
                            }
                        });
                    },
                }
            },
            {
                xtype: 'displayfield',
                fieldLabel: "商品名稱",
                name: 'product_name',
                labelWidth: 110,
                width: 400,
                id: 'product_name',
            },
            {
                xtype: 'textarea',
                fieldLabel: '文案內容',
                id: 'dr_content_text',
                labelWidth: 110,
                width: 400,
                name: 'dr_content_text',
                allowBlank: false
            },
            {
                xtype: 'textarea',
                fieldLabel: '需求描述',
                id: 'dr_description',
                labelWidth: 110,
                width: 400,
                name: 'dr_description',
                allowBlank: false
            },
            {
                xtype: 'textfield',
                fieldLabel: '素材路徑',
                id: 'dr_resource_path',
                labelWidth: 110,
                width: 400,
                name: 'dr_resource_path',
                regex: /^\\\\([0-9]|[a-z]|[A-Z]|[^\x00-\xff]|\\|●|_|▲|.)*$/,
                regexText: '格式不正確',
                allowBlank: false
            },
            {
                xtype: 'textfield',
                fieldLabel: '文件路徑',
                id: 'dr_document_path',
                labelWidth: 110,
                width: 400,
                name: 'dr_document_path',
                allowBlank: false,
                regex: /^\\\\([0-9]|[a-z]|[A-Z]|[^\x00-\xff]|\\|●|_|▲|.)*$/,
                regexText: '格式不正確'
            },
            {
                xtype: 'displayfield',
                fieldLabel: '建立日期',
                id: 'dr_created',
                labelWidth: 110,
                width: 400,
                name: 'dr_created',
                hidden: false
            },
            {
                xtype: 'displayfield',
                fieldLabel: '最後修改日期',
                labelWidth: 110,
                width: 400,
                id: 'dr_modified',
                name: 'dr_modified',
                hidden: false
            }
        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {               
                var form = this.up('form').getForm();
                if (Ext.getCmp('dr_id').getValue() != "") {
                    Ext.Msg.confirm("確認信息", "確定之後將會重新審核,是否確定?", function (btn) {
                        if (btn == 'no') {
                            editWin.close();
                        }
                        else {
                            if (form.isValid()) {
                                form.submit({
                                    params: {
                                        dr_id: Ext.htmlEncode(Ext.getCmp('dr_id').getValue()),
                                        dr_content_text: Ext.htmlEncode(Ext.getCmp('dr_content_text').getValue()),
                                        dr_description: Ext.htmlEncode(Ext.getCmp('dr_description').getValue()),
                                        dr_document_path: Ext.htmlEncode(Ext.getCmp('dr_document_path').getValue()),
                                        dr_resource_path: Ext.htmlEncode(Ext.getCmp('dr_resource_path').getValue()),
                                        dr_type: Ext.htmlEncode(Ext.getCmp('dr_type').getValue()),
                                        product_id: Ext.htmlEncode(Ext.getCmp('product_id').getValue())
                                    },
                                    success: function (form, action) {
                                        var result = Ext.decode(action.response.responseText);
                                        if (result.success) {
                                            if (result.msg == 1) {
                                                if (result.type == 1) {
                                                    Ext.Msg.alert(INFORMATION, "新增成功!");
                                                } else {
                                                    Ext.Msg.alert(INFORMATION, "編輯成功!");
                                                }
                                            }
                                            //else if (result.msg == 2) {
                                            //    Ext.Msg.alert(INFORMATION, "文案內容含有敏感詞彙!");
                                            //    return;
                                            //}
                                            else {
                                                Ext.Msg.alert(INFORMATION, "操作失敗!");
                                            }
                                            DesignRequentStore.load();
                                            editWin.close();
                                        }
                                        else {
                                            Ext.Msg.alert(INFORMATION, FAILURE);
                                        }
                                    },
                                    failure: function () {

                                        Ext.Msg.alert(INFORMATION, FAILURE);
                                    }
                                });
                            }
                        }
                    })
                }
                else {
                    if (form.isValid()) {
                        form.submit({
                            params: {
                                dr_id: Ext.htmlEncode(Ext.getCmp('dr_id').getValue()),
                                dr_content_text: Ext.htmlEncode(Ext.getCmp('dr_content_text').getValue()),
                                dr_description: Ext.htmlEncode(Ext.getCmp('dr_description').getValue()),
                                dr_document_path: Ext.htmlEncode(Ext.getCmp('dr_document_path').getValue()),
                                dr_resource_path: Ext.htmlEncode(Ext.getCmp('dr_resource_path').getValue()),
                                dr_type: Ext.htmlEncode(Ext.getCmp('dr_type').getValue()),
                                product_id: Ext.htmlEncode(Ext.getCmp('product_id').getValue())
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    if (result.msg == 1) {
                                        if (result.type == 1) {
                                            Ext.Msg.alert(INFORMATION, "新增成功!");
                                        } else {
                                            Ext.Msg.alert(INFORMATION, "編輯成功!");
                                        }
                                    }
                                    //else if (result.msg == 2) {
                                    //    Ext.Msg.alert(INFORMATION, "文案內容含有敏感詞彙!");
                                    //    return;
                                    //}
                                    else {
                                        Ext.Msg.alert(INFORMATION, "操作失敗!");
                                    }
                                    DesignRequentStore.load();
                                    editWin.close();
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            },
                            failure: function () {

                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        });
                    }
                }
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: '派工系統新增/編輯',
        id: 'editWin',
        iconCls: 'icon-user-edit',
        width: 450,
        height: 500,
        layout: 'fit',
        items: [editFrm],
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        constrain: true,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
            {
                type: 'close',
                qtip: CLOSE,
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp('editWin').destroy();
                        }
                        else {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            'show': function () {
                if (row != null) {
                    editFrm.getForm().loadRecord(row);
                    switch (row.data.dr_type) {
                        case 4:
                        case 5:
                            Ext.getCmp('product_id').show();
                            Ext.getCmp('product_name').show();
                            //var product_id = Ext.getCmp('product_id');
                            //product_id.allowBlank = false;
                            //product_id.show();
                            break;
                        default:
                            Ext.getCmp('product_id').hide();
                            Ext.getCmp('product_name').hide();
                            //var product_id = Ext.getCmp('product_id');
                            //product_id.allowBlank = true;
                            //product_id.hide();
                            break;
                    }
                    Ext.getCmp("dr_type").setRawValue(row.data.dr_type_tostring);
                    Ext.getCmp('product_id').setDisabled(true);
                    Ext.getCmp('dr_type').setDisabled(true);
                }
                else {
                    editFrm.getForm().reset();
                    Ext.getCmp("dr_created").hide();
                    Ext.getCmp("dr_modified").hide();
                    Ext.getCmp("dr_requester_id_name").hide();
                    Ext.getCmp('product_id').hide();
                    Ext.getCmp('product_name').hide();
                }
            }
        }
    });
    editWin.show();
};


DesigneeFunction = function (ID, STATUS) {
    //被指派需求執行者store
    Ext.define('assignToModel', {
        extend: 'Ext.data.Model',
        fields: [
             { name: "user_id", type: "int" },
            { name: "user_username", type: "string" }
        ]
    });

    var assignToUserStore = Ext.create('Ext.data.Store', {
        autoDestroy: true,
        model: 'assignToModel',
        remoteSort: false,
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: "/DispatchingSystem/GetDesign",
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });
    var editFrm1 = Ext.create('Ext.form.Panel', {
        id: 'editFrm1',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/DispatchingSystem/DesigneeSave',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 120 },
        items: [
            {
                xtype: 'combobox',
                fieldLabel: "被指派需求執行者",
                labelWidth: 110,
                id: 'dr_assign_to',
                name: 'dr_assign_to',
                width: 400,
                store: assignToUserStore,
                emptyText: '請選擇',
                allowBlank: false,
                forceSelection: true,
                typeAhead: true,
                triggerAction: 'all',
                displayField: 'user_username',
                valueField: 'user_id'
            }
        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            dr_id: ID,
                            dr_status: STATUS,
                            dr_assign_to: Ext.htmlEncode(Ext.getCmp("dr_assign_to").getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                if (result.msg == 2) {
                                    Ext.Msg.alert(INFORMATION, "沒有權限!");                                    
                                    editWin1.close();
                                }
                                else if (result.msg == 4) {
                                    Ext.Msg.alert(INFORMATION, "郵件發送失敗,請確認email后重新指派!");
                                    editWin1.close();
                                }
                                else
                                {
                                    Ext.Msg.alert(INFORMATION, "指派成功!");
                                    DesignRequentStore.load();
                                    editWin1.close();
                                }
                            }
                            else {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });
                }
            }
        }]
    });
    var editWin1 = Ext.create('Ext.window.Window', {
        title: '指派人員',
        id: 'editWin1',
        iconCls: 'icon-user-edit',
        width: 350,
        height: 200,
        layout: 'fit',
        items: [editFrm1],
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        constrain: true,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        margin:'5px 0px 0px 0px',
        closable: false,
        tools: [
            {
                type: 'close',
                qtip: CLOSE,
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp('editWin1').destroy();
                        }
                        else {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            'show': function () {
            }
        }
    });
    editWin1.show();
};