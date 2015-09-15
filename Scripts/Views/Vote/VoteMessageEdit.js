editFunction = function (row, store) {
    var editUserFrm = Ext.create('Ext.form.Panel', {
        id: 'editUserFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        labelWidth: 40,
        url: '/Vote/SaveVoteMessage',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [

            {
                fieldLabel: '留言編號',
                xtype: 'textfield',
                id: 'message_id',
                name: 'message_id',
                hidden: true
            },
             {
                 xtype: 'combobox',
                 editable: false,
                 fieldLabel: "文章標題",
                 labelWidth: 70,
                 queryMode: 'remote',//remote//local
                // queryMode: 'local',
                 id: 'article_id',
                 store: VoteArticleStore,
                 displayField: 'article_title',
                 valueField: 'article_id',
                 emptyText: '请选择',
                 lastQuery: '',
                 triggerAction: 'all',
                 allowBlank: false,
                 listeners: {
                     // delete the previous query in the beforequery event or set
                     // combo.lastQuery = null (this will reload the store the next time it expands)
                     beforequery: function (qe) {
                         VoteArticleStore.load();
                     }
                 }
                               
             },
            {
                xtype: 'textareafield',
                name: 'message_content',
                id: 'message_content',
                labelWidth: 70,
                maxLength:200,
                maxLengthText:'輸入文字在200字以內',
                allowBlank: false,
                submitValue: true,
                fieldLabel: '回覆內容 '
            }



            //,
           
            // {
            //     xtype: 'radiogroup',
            //     fieldLabel: "活動狀態",
            //     id: 'message_status',
            //     name: 'message_status',
            //     columns: 2,
            //     defaults: {
            //         flex: 1,
            //         name: 'status'
            //     },
            //     items: [
            //         {
            //             boxLabel: "啟用", id: 'yes', inputValue: '1', checked: true
            //         },
            //         {
            //             boxLabel: "禁用", id: 'no', inputValue: '0'
            //         }
            //     ],
            // }
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: SAVE,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: 'Loading...' });
                    form.submit({
                        params: {
                            message_id: Ext.getCmp('message_id').getValue(),
                            article_id: Ext.getCmp('article_id').getValue(),
                            message_content: Ext.htmlEncode(Ext.getCmp('message_content').getValue())
                            //,
                            //message_status: Ext.htmlEncode(Ext.getCmp('message_status').getValue().status)

                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            myMask.hide();
                            if (result.success == "true") {
                                Ext.Msg.alert("提示信息", result.msg);
                                editUserWin.close();
                                VoteMessageStore.load();

                            }
                            else {
                                Ext.Msg.alert("提示信息", result.msg);
                            }
                        },
                        failure: function (form, action) {
                            myMask.hide();
                            Ext.Msg.alert("提示信息", result.msg);
                        }
                    });
                }
            }
        }]
    });


    var editUserWin = Ext.create('Ext.window.Window', {
        id: 'editUserWin',
        width: 400,
        title: "留言內容",
        iconCls: 'icon-user-edit',
        iconCls: row ? "icon-user-edit" : "icon-user-add",
        layout: 'fit',
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        closable: false,
        items: [
                 editUserFrm
        ],
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        tools: [
         {
             type: 'close',
             qtip: "關閉",
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm("提示", "確定關閉?", function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editUserWin').destroy();
                         VoteMessageStore.destroy();
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
                if (row) {
                    editUserFrm.getForm().loadRecord(row); //如果是編輯的話

                }
                else {
                    editUserFrm.getForm().reset(); //如果是新增的話
                }
            }
        }
    });
    //if (row != null) {
    //    if (!row.data.message_status) {
    //        Ext.getCmp('no').setValue(true);
    //    }
    //    else {
    //        Ext.getCmp('yes').setValue(true);
    //    }
    //}
    editUserWin.show();
}
