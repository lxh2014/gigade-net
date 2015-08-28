var CallidForm;
var pageSize = 25;
/**********************************************************************回覆訂單問題和意見主頁面**************************************************************************************/
replyFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        url: '/OrderManage/Reply',
        layout: 'anchor',
        defaults: {
            bodyStyle: 'padding:10px',
            anchor: "95%",
            msgTarget: "side"
        },
        items: [
           {
               xtype: 'displayfield',
               fieldLabel: 'ID',
               id: 'cancel_id',
               name: 'cancel_id',
               hidden: true
           },
         {
             xtype: 'displayfield',
             fieldLabel: NAME,
             id: 'order_name',
             name: 'order_name'
         },
          {
              xtype: 'displayfield',
              fieldLabel: CANCELTYPE,
              id: 'scancel_type',
              name: 'scancel_type'
          },
          {
              xtype: 'displayfield',
              fieldLabel: QUESTIONDATE,
              id: 'cancel_createdate',
              name: 'cancel_createdate'
          },
          {
              xtype: 'displayfield',
              fieldLabel: MOBILE,
              id: 'order_mobile',
              name: 'order_mobile'
          },
          {
              xtype: 'displayfield',
              fieldLabel: EMAIL,
              id: 'user_email',
              name: 'user_email'
          },
         {
             xtype: 'displayfield',
             fieldLabel: ORDERID,
             id: 'order_id',
             name: 'order_id'
         },
         {
             xtype: 'textareafield',
             fieldLabel: QUESTIONCONTENT,
             id: 'cancel_content',
             readOnly: true,
             autoScroll: true,
             name: 'cancel_content'
         },
         {
             xtype: 'textareafield',
             fieldLabel: REPLY,
             id: 'response',
             autoScroll: true,
             allowBlank: false,
             name: 'response'
         }
        ],
        buttons: [{
            disabled: true,
            formBind: true,
            text: REPLY,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            cancel_id: Ext.htmlEncode(Ext.getCmp('cancel_id').getValue()),
                            question_email: Ext.htmlEncode(Ext.getCmp('user_email').getValue()),
                            response: Ext.htmlEncode(Ext.getCmp('response').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, REPLYSUCCESS + result.msg);
                                CancelMsgStore.load();
                                editWin.close();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, REPLYFAILURE + result.msg);
                                CancelMsgStore.load();
                                editWin.close();
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, REPLYFAILURE + result.msg);
                            CancelMsgStore.load();
                            editWin.close();
                        }
                    });

                }

            }
        }]

    });
    var editWin = Ext.create('Ext.window.Window', {
        title: REPLYTITLE,
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 400,
        y: 100,
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
             qtip: QTIP,
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
                       editFrm.getForm().loadRecord(row);

                    }
                }
    });
    editWin.show();
}