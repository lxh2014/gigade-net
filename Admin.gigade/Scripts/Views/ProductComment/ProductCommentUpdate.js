updateFunction = function (row, store) {
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
    var CommentSatisfyStore = Ext.create('Ext.data.Store', {
        fields: ['txt', 'value'],
        data: [
        { "txt": '非常滿意(5分)', "value": "5" },
        { "txt": '滿意(4分)', "value": "4" },
        { "txt": '一般(3分)', "value": "3" },
        { "txt": '不滿意(2分)', "value": "2" },
        { "txt": '非常不滿意(1分)', "value": "1" },
        
        ]
    });

    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/ProductComment/ProductCommentSatisfySave',
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
              xtype: 'displayfield',
              name: 'create_time',
              id: 'create_time',
              submitValue: true,
              allowBlank: false,
              fieldLabel: '評價時間'
          },
          {
              xtype: 'combobox',
              fieldLabel: '商品描述相符度',
              editable: false,
              id: 'product_desc',
              name: 'product_desc',
              store: CommentSatisfyStore,
              valueField: 'value',
              displayField: 'txt',
              //value: '0',
          },
          {
              xtype: 'combobox',
              fieldLabel: '商品質量滿意度',
              editable: false,
              id: 'seller_server',
              name: 'seller_server',
              store: CommentSatisfyStore,
              valueField: 'value',
              displayField: 'txt',
              //value: '0',
          },
          {
              xtype: 'combobox',
              fieldLabel: '配送速度滿意度',
              editable: false,
              id: 'logistics_deliver',
              name: 'logistics_deliver',
              store: CommentSatisfyStore,
              valueField: 'value',
              displayField: 'txt',
              //value: '0',
          },
          {
              xtype: 'combobox',
              fieldLabel: '網站整體滿意度',
              editable: false,
              id: 'web_server',
              name: 'web_server',
              store: CommentSatisfyStore,
              valueField: 'value',
              displayField: 'txt',
              //value: '0',
          },

      
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: '保存',
            handler: function () {
                var form = this.up('form').getForm();
                
                var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                myMask.show();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            comment_id: Ext.htmlEncode(Ext.getCmp('comment_id').getValue()),
                            logistics_deliver: Ext.getCmp('logistics_deliver').getValue(),
                            web_server: Ext.getCmp('web_server').getValue(),
                            seller_server: Ext.getCmp('seller_server').getValue(),
                            product_desc: Ext.getCmp('product_desc').getValue(),
                            old_logistics_deliver: row.data.logistics_deliver,
                            old_web_server: row.data.web_server,
                            old_seller_server: row.data.seller_server,
                            old_product_desc: row.data.product_desc,
                           
                        },
                        success: function (form, action) {
                            myMask.hide();
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, "保存成功 ");
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
        title: '評價編輯',
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