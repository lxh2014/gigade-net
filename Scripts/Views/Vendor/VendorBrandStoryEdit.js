EditFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        layout: 'anchor',
        plain: true,
        frame: true,
        constrain: true,
        autoScroll: true,
        url: '/Vendor/AddVendorBrandStory',
        defaults: { anchor: "99%" },
        items: [
            {
                xtype: 'fieldcontainer',
                layout: 'column',
                height: 40,
                items: [
                        {
                            xtype: 'displayfield',
                            fieldLabel: "品牌編號",
                            id: 'brand_id',
                            name: 'brand_id',
                            labelWidth: 65,
                            width: 100
                        },
                         {
                             xtype: 'displayfield',
                             fieldLabel: "| 品牌名稱",
                             id: 'brand_name',
                             name: 'brand_name',
                             labelWidth: 80,
                             width: 270
                         }
                ]
            },
            {
                xtype: 'displayfield',
                fieldLabel: '品牌故事文字'
            }, {
                xtype: 'textareafield',
                id: 'brand_story_text',
                name: 'brand_story_text',
                height: 250,
                anchor: '100%',
                allowBlank: false,
                autoScroll: true
            }
        ],
        buttons: [{
            text: '保存',
            formBind: true,
            disabled: false,
            handler: function () {
                var form = this.up('form').getForm();
                var text = Ext.getCmp('brand_story_text').getValue().toString();
                var str = text.replace(/ /g, '')
                if (str.length == 0) {
                    Ext.Msg.alert(INFORMATION, "不能為空字符串！");
                    return;
                }
                form.submit({
                    params: {
                        brandid: Ext.getCmp('brand_id').getValue(),
                        brandName: Ext.getCmp('brand_name').getValue(),
                        brandStoryText: Ext.getCmp('brand_story_text').getValue()
                    },
                    success: function (form, action) {
                        var result = Ext.decode(action.response.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, result.msg);
                            VendorBrandStory.load();
                            editWin.close();
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, result.msg);
                            VendorBrandStory.load();
                            editWin.close();
                        }
                    },
                    failure: function (form, action) {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                        VendorBrandStory.load();
                        editWin.close();
                    }
                })
            }
        }

        ]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: '品牌故事內容',
        id: 'editWin',
        width: 430,
        height: 410,
        layout: 'fit',
        iconCls: "icon-user-edit",
        bodyStyle: 'padding:5px 5px 5px 5px',
        items: [editFrm],
        constrain: true,
        labelWidth: 60,
        closeAction: 'destroy',
        modal: true,
        closable: false,
        tools: [
             {
                 type: 'close',
                 qtip: '是否關閉',
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
             }],
        listeners: {
            'show': function () {
                editFrm.getForm().loadRecord(row); //如果是編輯的話
            }
        }
    });
    editWin.show();
}
