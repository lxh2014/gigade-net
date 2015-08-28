editFunction = function (row, store) {
    //参数：mystr传入的字符串
    //返回：字符串mystr
    function trim(mystr) {
        while ((mystr.indexOf(" ") == 0) && (mystr.length > 1)) {
            mystr = mystr.substring(1, mystr.length);
        }//去除前面空格
        while ((mystr.lastIndexOf(" ") == mystr.length - 1) && (mystr.length > 1)) {
            mystr = mystr.substring(0, mystr.length - 1);
        }//去除后面空格
        if (mystr == " ") {
            mystr = "";
        }
        return mystr;
    }

    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/ProductComment/ProductCommentSave',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [{
            xtype: 'textfield',
            fieldLabel: '編號',
            id: 'comment_id',
            name: 'comment_id',
            submitValue: true,
            hidden: true
        },
         {
             xtype: 'displayfield',
             name: 'product_name',
             id: 'product_name',
             submitValue: true,
             allowBlank: false,
             fieldLabel: '商品名稱'
         },
          {
              xtype: 'displayfield',
              name: 'comment_info',
              id: 'comment_info',
              submitValue: true,
              allowBlank: false,
              fieldLabel: '留言內容'
          },
          {
              xtype: 'radiogroup',
              fieldLabel: '發送郵件',
              id: 'send_mail',
              name: 'send_mail',
              defaults: {
                  flex: 1,
                  name: 'send_mail'
              },
              items: [
                  {
                      id: 'id_yes',
                      inputValue: 0,
                      checked:true,
                      boxLabel:'是'
                  },
                  {
                      id: 'id_no',
                      inputValue: 1,
                      boxLabel: '否'
                  }
              ]
          },
                  {
                      xtype: 'radiogroup',
                      fieldLabel: '回覆內容是否顯示',
                      id: 'answer_is_show',
                      name: 'answer_is_show',
                      defaults: {
                          flex: 1,
                          name: 'answer_is_show'
                      },
                      items: [
                          {
                              id: 'answer_show',
                              inputValue: 1,
                              checked: true,
                              boxLabel: '是'
                          },
                          {
                              id: 'answer_hide',
                              inputValue: 0,
                              boxLabel: '否'
                          }
                      ]
                  },
        {
            xtype: 'textareafield',
            name: 'comment_answer',
            id: 'comment_answer',
            submitValue: true,
            allowBlank: false,
            maxLength:200,
            fieldLabel: '回覆內容'
        },
      
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: '回覆',
            handler: function () {
                var form = this.up('form').getForm();
                var comment = Ext.getCmp('comment_answer').getValue();
                if ((trim(comment) == ""))
                {
                    Ext.Msg.alert("提示信息","輸入內容不能為空");
                    return;
                }

                var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                myMask.show();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            comment_id: Ext.htmlEncode(Ext.getCmp('comment_id').getValue()),
                            comment_answer: Ext.getCmp('comment_answer').getValue(),
                            user_email: row.data.user_email,
                            user_name: row.data.user_name,
                            product_name: row.data.product_name,
                            send_mail: Ext.getCmp('send_mail').getValue().send_mail,
                            answer_is_show: Ext.getCmp('answer_is_show').getValue().comment_is_show,
                            old_answer_is_show: row.data.answer_is_show,
                            old_comment_answer: row.data.comment_answer,
                            comment_detail_id: row.data.comment_detail_id,
                            isReplay: row.data.isReplay
                        },
                        success: function (form, action) {
                            myMask.hide();
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                if (result.msg == "0")
                                {
                                    Ext.Msg.alert(INFORMATION, "保存成功 ");
                                }
                                else if (result.msg == "1")
                                {
                                    Ext.Msg.alert(INFORMATION, "保存成功,郵件發送成功！ ");
                                }
                                else if (result.msg == "2") {
                                    Ext.Msg.alert(INFORMATION, "保存成功,郵件發送失敗！ ");
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                }
                                ProductCommentStore.load();
                                editWin.close();

                            }
                            else {
                                Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                ProductCommentStore.load();
                                editWin.close();
                            }
                        },
                        failure: function (form, action) {
                            myMask.hide();
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, "保存失敗! ");
                            ProductCommentStore.load();
                            editWin.close();
                        }
                    });
                }
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: '留言回覆',
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 400,
        y: 100,
        height:300,
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
         }
        ],
        listeners: {
            'show': function () {
                editFrm.getForm().loadRecord(row);
                if (row.data.answer_is_show == 0)
                {
                    Ext.getCmp('answer_hide').setValue(true);
                }
                else if (row.data.answer_is_show == 1)
                {
                    Ext.getCmp('answer_show').setValue(true);
                }
            }
        }
    });
    editWin.show();
}