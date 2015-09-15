
function editFunction(rowID) {
    var row;
    edit_ContactUsStore.load({
        params: { relation_id: rowID },
        callback: function () {
            row = edit_ContactUsStore.getAt(0);
            editWins.show();
        }
    });
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        url: '/Service/update',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: "流水號",
                id: 'question_id',
                name: 'question_id',
                hidden: true,
                submitValue: true
            }, {
                xtype: 'displayfield',
                fieldLabel: "會員ID",
                id: 'user_id',
                name: 'user_id',
                hidden: true,
                submitValue: true
            },
             {
                 xtype: 'displayfield',
                 fieldLabel: "姓名",
                 id: 'question_username',
                 name: 'question_username',
                 submitValue: true
             },
              {
                  xtype: 'displayfield',
                  fieldLabel: '公司',
                  id: 'question_company',
                  name: 'question_company',
                  allowBlank: false
              },
            {
                xtype: 'displayfield',
                fieldLabel: "電話",
                id: 'question_phone',
                name: 'question_phone',
                submitValue: true
            },
             {
                 xtype: 'displayfield',
                 fieldLabel: '信箱',
                 id: 'question_email',
                 name: 'question_email',
                 allowBlank: false
             },
             {
                 xtype: 'displayfield',
                 fieldLabel: "建立日期",
                 id: 'question_createdate',
                 name: 'question_createdate',
                 submitValue: true
             },
            {
                xtype: 'displayfield',
                fieldLabel: "問題類別",
                id: 'question_problem_name',
                name: 'question_problem_name',
                submitValue: true
            },
             {
                 xtype: 'displayfield',
                 fieldLabel: "類型",
                 id: 'question_type_name',
                 name: 'question_type_name',
                 submitValue: true
             },
            {
                xtype: 'displayfield',
                fieldLabel: "信箱",
                id: 'question_email',
                name: 'question_email',
                submitValue: true
            },
             {
                 xtype: 'displayfield',
                 fieldLabel: "來源IP",
                 id: 'question_ipfrom',
                 name: 'question_ipfrom',
                 submitValue: true
             },
             {
                 xtype: 'fieldcontainer',
                 combineErrors: true,
                 hidden: true,
                 layout: 'hbox',
                 items: [
                   { xtype: 'displayfield', value: "狀態:" },
                   {
                       xtype: 'radiofield',
                       boxLabel: '待回覆',
                       margin: '0 0 0 37',
                       name: 'state',
                       id: 'state1'
                   },
                      {
                          xtype: 'radiofield',
                          boxLabel: '已回覆',
                          name: 'state',
                          id: 'state2'
                      },
                      {
                          xtype: 'radiofield',
                          boxLabel: '已處理(不會寄出通知信) ',
                          name: 'state',
                          id: 'state3'
                      }
                 ]
             },
             {
                 xtype: 'fieldcontainer',
                 combineErrors: true,
                 layout: 'hbox',
                 items: [
                   { xtype: 'displayfield', value: "回覆方式:" },
                   {
                       xtype: 'checkboxfield',
                       boxLabel: 'E-mail',
                       margin: '0 0 0 25',
                       name: 'reply',
                       id: 'reply1'
                   },
                   {
                       xtype: 'checkboxfield',
                       boxLabel: '簡訊 ',
                       margin: '0 0 0 10',
                       name: 'reply',
                       id: 'reply2'
                   },
                   {
                       xtype: 'checkboxfield',
                       boxLabel: '電話(希望回覆時間',
                       name: 'reply',
                       margin: '0 0 0 10',
                       id: 'reply3'
                   },

                        {
                            xtype: 'radiofield',
                            boxLabel: '上午時段 : 9點 -12點 ',
                            name: 'phone',
                            id: 'phone1'
                        },
                           {
                               xtype: 'radiofield',
                               boxLabel: '下午時段 : 2點 -6點',
                               name: 'phone',
                               id: 'phone2'
                           },
                           {
                               xtype: 'radiofield',
                               boxLabel: '不限時段) ',
                               name: 'phone',
                               id: 'phone3'
                           }
                 ]
             },
              {
                  xtype: 'displayfield',
                  fieldLabel: "內容",
                  id: 'question_content',
                  name: 'question_content',
                  submitValue: true
              },
               {
                   xtype: 'textarea',
                   fieldLabel: "回覆",
                   id: 'content_reply',
                   name: 'content_reply',
                   submitValue: true
               }
        ],
        buttons: [
              {
                  text: "回覆記錄",
                  id: 'reply_record_id',
                  hidden: true,
                  handler: function () {
                      var question_id = Ext.getCmp('question_id').getValue();
                      TranToMore("/Service/ContactRecord", question_id);
                  }
              },
            {
                formBind: true,
                disabled: true,
                text: "回覆",
                id:'btn_reply',
                vtype: 'submit',
                handler: function () {
                    var form = this.up('form').getForm();
                    if (Ext.getCmp('content_reply').getValue().trim() == "" && Ext.getCmp('state3').getValue() == false) {
                        Ext.Msg.alert(INFORMATION, "回覆內容不能為空!");
                        return;
                    }
                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM_REPLY, function (btn) {
                        if (btn == "yes") {
                            var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "提交回覆中,請稍等......" });
                            myMask.show();
                            if (form.isValid()) {
                                Ext.getCmp('btn_reply').disable();
                                form.submit({
                                    //waitMsg: '提交回復中,請稍等......',
                                    //waitTitle: '提示',
                                    //timeout: 9000000,
                                    params: {
                                        question_id: Ext.getCmp('question_id').getValue(),
                                        question_username: Ext.getCmp('question_username').getValue(),
                                        question_phone: Ext.getCmp('question_phone').getValue(),
                                        question_createdate: Ext.getCmp('question_createdate').getValue(),
                                        question_problem_name: Ext.getCmp('question_problem_name').getValue(),
                                        question_type_name: Ext.getCmp('question_type_name').getValue(),
                                        question_email: Ext.getCmp('question_email').getValue(),
                                        question_ipfrom: Ext.getCmp('question_ipfrom').getValue(),
                                        state1: Ext.getCmp('state1').getValue(),
                                        state2: Ext.getCmp('state2').getValue(),
                                        state3: Ext.getCmp('state3').getValue(),
                                        reply1: Ext.getCmp('reply1').getValue(),
                                        reply2: Ext.getCmp('reply2').getValue(),
                                        reply3: Ext.getCmp('reply3').getValue(),
                                        phone1: Ext.getCmp('phone1').getValue(),
                                        phone2: Ext.getCmp('phone2').getValue(),
                                        phone3: Ext.getCmp('phone3').getValue(),
                                        question_content: Ext.getCmp('question_content').getValue(),
                                        content_reply: Ext.getCmp('content_reply').getValue()
                                    },
                                    success: function (form, action) {
                                        myMask.hide();
                                        var result = Ext.decode(action.response.responseText);
                                        if (result.success) {
                                            Ext.Msg.alert(INFORMATION, SUCCESS);
                                            ContactUsStore.load();
                                            editWins.close();
                                        }
                                        else {
                                            Ext.Msg.alert(INFORMATION, FAILURE);
                                            ContactUsStore.load();
                                            editWins.close();
                                        }
                                    },
                                    failure: function (form, action) {
                                        var result = Ext.decode(action.response.responseText);
                                        Ext.Msg.alert(INFORMATION, FAILURE);
                                        ContactUsStore.load();
                                        editWins.close();
                                    }
                                });
                            }
                        }
                        else {
                            return false;
                        }
                    });
                }
            }
        ]
    });
    var editWins = Ext.create('Ext.window.Window', {
        title: "【聯絡客服】內容",
        iconCls: row ? 'icon-user-edit' : 'icon-user-add',
        id: 'editWins',
        width: 1000,
        height: 600,
        layout: 'fit',
        items: [editFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
            {
                type: 'close',
                qtip: "關閉",
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp('editWins').destroy();
                        } else {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            'show': function () {

                editFrm.getForm().loadRecord(row);
                //值進行設置
                if (row.data.user_id == 0) {
                    Ext.getCmp('user_id').hide();
                }
                else {
                    Ext.getCmp('user_id').show();
                }
                if (row.data.question_status == 0) {
                    Ext.getCmp('state1').setValue(true);

                }
                else if (row.data.question_status == 1) {
                    Ext.getCmp('state2').setValue(true);
                    Ext.getCmp('reply_record_id').show();
                }
                else if (row.data.question_status == 2) {
                    Ext.getCmp('state3').setValue(true);
                    Ext.getCmp('reply_record_id').show();
                }
                Ext.getCmp('state1').setDisabled(true);
                var replaycontent = row.data.question_reply;
                var result = replaycontent.split('|');

                Ext.getCmp('reply1').setValue(result[0]);
                Ext.getCmp('reply2').setValue(result[1]);
                Ext.getCmp('reply3').setValue(result[2]);
                if (result[2]) {
                    if (row.data.question_reply_time == '1') {
                        Ext.getCmp('phone1').setValue(true);
                    }
                    else if (row.data.question_reply_time == '2') {
                        Ext.getCmp('phone2').setValue(true);
                    }
                    else if (row.data.question_reply_time == '3') {
                        Ext.getCmp('phone3').setValue(true);
                    }
                }
                Ext.getCmp('reply1').setDisabled(true);
                Ext.getCmp('reply2').setDisabled(true);
                Ext.getCmp('reply3').setDisabled(true);
                Ext.getCmp('phone1').setDisabled(true);
                Ext.getCmp('phone2').setDisabled(true);
                Ext.getCmp('phone3').setDisabled(true);

            }
        }
    });
}

function TranToMore(url, qid) {
    var record = "回覆記錄";
    var urlTran = url + '?qid=' + qid;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#groupDetail');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'groupDetail',
        title: record,
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}