editFunction = function (Row, store) {
    ArticleStore.load();

    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/Vote/SaveVoteDetail',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },
        items: [
            {
                xtype: 'textfield',
                fieldLabel: 'ID',
                id: 'vote_id',
                name: 'vote_id',
                hidden: true
            },
            {
                xtype: 'combobox',
                allowBlank: false,
                editable: false,
                typeAhead: true,
                forceSelection: false,
                fieldLabel: "文章名稱",
                id: 'article_id',
                name: 'article_id',
                hiddenName: 'article_id',
                colName: 'article_id',
                lastQuery: '',
                store: ArticleStore,
                displayField: 'article_title',
                valueField: 'article_id',
                emptyText: "請選擇"
            },
            {
                xtype: 'textfield',
                fieldLabel: "會員編號",
                allowBlank: false,
                id: 'user_id',
                name: 'user_id',
                maxLength: "100"
            }
            //,
            //{
            //    xtype: 'radiogroup',
            //    //hidden: false,
            //    id: 'vote_status',
            //    name: 'vote_status',
            //    fieldLabel: "是否啟用",
            //    colName: 'vote_status',
            //    width: 400,
            //    defaults: {
            //        name: 'vote_status',
            //        margin: '0 8 0 0'
            //    },
            //    columns: 2,
            //    vertical: true,
            //    items: [
            //        {
            //            boxLabel: "啟用",
            //            id: 'isStatus',
            //            inputValue: '1',
            //            checked: true
            //        },
            //        {
            //            boxLabel: "不啟用",
            //            id: 'noStatus',
            //            inputValue: '0'
            //        }
            //    ]
            //}
        ],
        buttons: [
            {
                text: SAVE,
                formBind: true,
                disabled: true,
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        var oldStatus = 0; //修改時原數據的狀態為不啟用，要修改為啟用時，並且當前啟用值大於等於限制值，並且值存在時才提示
                        if (Row) {
                            oldStatus = Row.data.vote_status;
                        }
                        Ext.Ajax.request({
                            url: "/Vote/SaveVoteDetail",
                            method: 'post',
                            async: false, //true為異步，false為同步
                            params: {
                                vote_id: Ext.getCmp("vote_id").getValue(),
                                article_id: Ext.getCmp("article_id").getValue(),
                                user_id: Ext.getCmp("user_id").getValue()
                            },
                            success: function (form, action) {
                                var result = Ext.decode(form.responseText);
                                if (result.success) {
                                    Ext.Msg.alert("提示信息", result.msg);
                                    editWin.close();
                                    VoteDetailStore.load();
                                } else {
                                    Ext.Msg.alert("提示信息",result.msg);
                                }
                            },
                            failure: function (form, action) {
                                Ext.Msg.alert("提示信息", result.msg);
                            }
                        });
                        //if (Ext.getCmp("vote_status").getValue().content_status == "1" && oldStatus == 0 && list_status_num >= content_status_num && content_status_num != 0) {//當選擇啟用並且已啟用數大於或等於最大限制值時提示是否執行
                        //    Ext.Msg.confirm(CONFIRM, Ext.String.format(STATUSTIP), function (btn) {
                        //        if (btn == 'yes') {
                        //            form.submit({
                        //                params: {
                        //                    rowid: Ext.htmlEncode(Ext.getCmp("content_id").getValue()),
                        //                    page_id: Ext.htmlEncode(Ext.getCmp("page_id").getValue()),
                        //                    area_id: Ext.htmlEncode(Ext.getCmp("area_id").getValue()),
                        //                    content_default: Ext.htmlEncode(Ext.getCmp("content_default").getValue().content_default),
                        //                    content_title: Ext.htmlEncode(Ext.getCmp("content_title").getValue())
                        //                },
                        //                success: function (form, action) {
                        //                    var result = Ext.decode(action.response.responseText);
                        //                    if (result.success) {
                        //                        if (result.msg != undefined) {
                        //                            Ext.Msg.alert(INFORMATION, result.msg);
                        //                        } else {
                        //                            Ext.Msg.alert(INFORMATION, SUCCESS);
                        //                            store.load();
                        //                            editWin.close();
                        //                        }
                        //                    } else {
                        //                        Ext.Msg.alert(INFORMATION, FAILURE);
                        //                    }
                        //                },
                        //                failure: function () {
                        //                    Ext.Msg.alert(INFORMATION, FAILURE);
                        //                }
                        //            });
                        //        } else {
                        //            return;
                        //        }
                        //    });
                        //} else {
                        //    form.submit({
                        //        params: {
                        //            //rowid: Ext.htmlEncode(Ext.getCmp("content_id").getValue()),
                        //            //page_id: Ext.htmlEncode(Ext.getCmp("page_id").getValue()),
                        //            //area_id: Ext.htmlEncode(Ext.getCmp("area_id").getValue()),
                        //            //content_default: Ext.htmlEncode(Ext.getCmp("content_default").getValue().content_default),
                        //            //content_title: Ext.htmlEncode(Ext.getCmp("content_title").getValue())
                        //        },
                        //        success: function (form, action) {
                        //            var result = Ext.decode(action.response.responseText);
                        //            if (result.success) {
                        //                if (result.msg != undefined) {
                        //                    Ext.Msg.alert(INFORMATION, result.msg);
                        //                } else {
                        //                    Ext.Msg.alert(INFORMATION, SUCCESS);
                        //                    store.load();
                        //                    editWin.close();
                        //                }
                        //            } else {
                        //                Ext.Msg.alert(INFORMATION, FAILURE);
                        //            }
                        //        },
                        //        failure: function () {
                        //            Ext.Msg.alert(INFORMATION, FAILURE);
                        //        }
                        //    });
                        //}
                    }
                }
            }
        ]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: "投票",
        id: 'editWin',
        iconCls: Row ? "icon-user-edit" : "icon-user-add",
        width: 500,
        layout: 'fit',
        items: [editFrm],
        constrain: true, //束縛窗口在框架內
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
            {
                type: 'close',
                qtip: "關閉窗口",
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm("確認", "是否確定關閉窗口?", function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp('editWin').destroy();
                        } else {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            'show': function () {
                if (Row) {
                    editFrm.getForm().loadRecord(Row); //如果是編輯的話
                    initForm(Row);
                } else {
                    editFrm.getForm().reset(); //如果是編輯的話
                }
            }
        }
    });
    editWin.show();
}