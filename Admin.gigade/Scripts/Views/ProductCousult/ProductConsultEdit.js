var ischecked = 0;
editFunction = function (rowID, store) {
    var row = null;
    if (rowID != null) {
        edit_ProductConsultStore.load({
            params: { relation_id: rowID },
            callback: function () {
                row = edit_ProductConsultStore.getAt(0);
                editWin.show();
            }
        });
    }
    else {
        editWin.show();
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
        url: '/ProductConsult/SaveProductConsultAnswer',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [{
            xtype: 'textfield',
            fieldLabel: '諮詢編號',
            id: 'consult_id',
            name: 'consult_id',
            submitValue: true,
            hidden: true
        },
         {
             xtype: 'displayfield',
             fieldLabel: '諮詢時間',
             id: 'create_date',
             name: 'create_date',
             submitValue: true
         },
         {
             xtype: 'displayfield',
             fieldLabel: '商品編號',
             id: 'product_id',
             name: 'product_id',
             submitValue: true
         },
          {
              xtype: 'displayfield',
              fieldLabel: '詳情鏈接',
              id: 'consult_url',
              name: 'consult_url',
              submitValue: true,
              hidden: true
          },
          //{
          //    xtype: 'displayfield',
          //    fieldLabel: '是否發送郵件',
          //    id: 'is_sendEmail',
          //    name: 'is_sendEmail',
          //    submitValue: true,
          //    hidden: true
          //},
          {
              xtype: 'displayfield',
              fieldLabel: '郵箱',
              id: 'user_email',
              name: 'user_email',
              submitValue: true
          },
           {
               xtype: 'displayfield',
               fieldLabel: '子商品編號',
               id: 'item_id',
               name: 'item_id',
               submitValue: true,
               hidden: true
           },
            {
                xtype: 'displayfield',
                fieldLabel: '規格編號',
                id: 'spec_id',
                name: 'spec_id',
                submitValue: true,
                hidden: true
            },
          {
              xtype: 'displayfield',
              fieldLabel: '商品鏈接',
              id: 'product_url',
              name: 'product_url',
              submitValue: true,
              hidden: true
          },
        {
            xtype: 'displayfield',
            fieldLabel: '用戶名稱',
            id: 'user_name',
            name: 'user_name',
            submitValue: true
        },
         {
             xtype: 'displayfield',
             name: 'product_name',
             id: 'product_name',
             submitValue: true,
             fieldLabel: '商品名稱',
         },
         {
             xtype: 'radiogroup',
             fieldLabel: '諮詢類型',
             id: "zixunType",
             name: 'zixunType',
             defaults: {
                 name: 'zixunType',
             },
             columns: 3,
             vertical: true,
             items: [
                 {
                     boxLabel: '商品諮詢', id: "consultType1", inputValue: '1', checked: false, listeners: {
                         change: function (radio, newValue, oldValue) {
                             if (newValue) {
                                 if (ischecked != 0) {
                                     Ext.MessageBox.confirm(CONFIRM, "更改類型會向相關負責人發送郵件，確認更改？", function (btn) {
                                         if (btn == "yes") {
                                             var zixunType = Ext.getCmp("zixunType").getValue().zixunType;
                                             Ext.Ajax.request({
                                                 url: '/ProductConsult/SendMailByGroup',
                                                 method: 'post',
                                                 params: {
                                                     zixunType: zixunType,
                                                     consult_id: row.data.consult_id,
                                                     user_name: row.data.user_name,
                                                     user_email: row.data.user_email,
                                                     create_date: row.data.create_date,
                                                     consult_info: row.data.consult_info,
                                                     ckShopClass: row.data.prod_classify == 10 ? "ckShopClass1" : "ckShopClass2"
                                                 },
                                                 success: function (form, action) {
                                                     ProductConsultStore.load();
                                                     ischecked = 0;
                                                     Ext.Msg.alert("提示信息", "操作成功！");
                                                     editWin.close();

                                                 },
                                                 failure: function () {
                                                     Ext.Msg.alert(INFORMATION, FAILURE);
                                                 }
                                             });
                                         }
                                         else {
                                             var zTemp = row.data.consult_type;
                                             ischecked = 0;
                                             switch (zTemp) {
                                                 case 1:
                                                     Ext.getCmp("consultType1").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                                 case 2:
                                                     Ext.getCmp("consultType2").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                                 case 3:
                                                     Ext.getCmp("consultType3").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                                 case 4:
                                                     Ext.getCmp("consultType4").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                                 case 5:
                                                     Ext.getCmp("consultType5").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                             }
                                         }
                                     });
                                 }
                             }

                         }
                     }
                 },
                 {
                     boxLabel: '庫存及配送', id: "consultType2", inputValue: '2', checked: false,
                     listeners: {
                         change: function (radio, newValue, oldValue) {
                             if (newValue) {
                                 if (ischecked != 0) {
                                     Ext.MessageBox.confirm(CONFIRM, "更改類型會向相關負責人發送郵件，確認更改？", function (btn) {
                                         if (btn == "yes") {
                                             var zixunType = Ext.getCmp("zixunType").getValue().zixunType;
                                             Ext.Ajax.request({
                                                 url: '/ProductConsult/SendMailByGroup',
                                                 method: 'post',
                                                 params: {
                                                     zixunType: zixunType,
                                                     consult_id: row.data.consult_id,
                                                     user_name: row.data.user_name,
                                                     user_email: row.data.user_email,
                                                     parameterName: row.data.parameterName,
                                                     consult_info: row.data.consult_info,
                                                     create_date: row.data.create_date,
                                                     ckShopClass: row.data.prod_classify == 10 ? "ckShopClass1" : "ckShopClass2"
                                                 },
                                                 success: function (form, action) {
                                                     ProductConsultStore.load();
                                                     ischecked = 0;
                                                     Ext.Msg.alert("提示信息", "操作成功！");
                                                     editWin.close();
                                                 },
                                                 failure: function () {
                                                     Ext.Msg.alert(INFORMATION, FAILURE);
                                                 }
                                             });
                                         }
                                         else {
                                             ischecked = 0;
                                             var zTemp = row.data.consult_type;
                                             switch (zTemp) {
                                                 case 1:
                                                     Ext.getCmp("consultType1").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                                 case 2:
                                                     Ext.getCmp("consultType2").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                                 case 3:
                                                     Ext.getCmp("consultType3").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                                 case 4:
                                                     Ext.getCmp("consultType4").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                                 case 5:
                                                     Ext.getCmp("consultType5").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                             }

                                         }
                                     });
                                 }
                             }



                         }
                     }
                 },
                 {
                     boxLabel: '支付問題', id: "consultType3", inputValue: '3', checked: false,
                     listeners: {
                         change: function (radio, newValue, oldValue) {
                             if (newValue) {
                                 if (ischecked != 0) {
                                     Ext.MessageBox.confirm(CONFIRM, "更改類型會向相關負責人發送郵件，確認更改？", function (btn) {
                                         if (btn == "yes") {
                                             var zixunType = Ext.getCmp("zixunType").getValue().zixunType;
                                             Ext.Ajax.request({
                                                 url: '/ProductConsult/SendMailByGroup',
                                                 method: 'post',
                                                 params: {
                                                     zixunType: zixunType,
                                                     consult_id: row.data.consult_id,
                                                     user_name: row.data.user_name,
                                                     user_email: row.data.user_email,
                                                     parameterName: row.data.parameterName,
                                                     consult_info: row.data.consult_info,
                                                     create_date: row.data.create_date,
                                                     ckShopClass: row.data.prod_classify == 10 ? "ckShopClass1" : "ckShopClass2"
                                                 },
                                                 success: function (form, action) {
                                                     ProductConsultStore.load();
                                                     ischecked = 0;
                                                     Ext.Msg.alert("提示信息", "操作成功！");
                                                     editWin.close();
                                                 },
                                                 failure: function () {
                                                     Ext.Msg.alert(INFORMATION, FAILURE);
                                                 }
                                             });
                                         }
                                         else {
                                             ischecked = 0;
                                             var zTemp = row.data.consult_type;
                                             switch (zTemp) {
                                                 case 1:
                                                     Ext.getCmp("consultType1").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                                 case 2:
                                                     Ext.getCmp("consultType2").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                                 case 3:
                                                     Ext.getCmp("consultType3").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                                 case 4:
                                                     Ext.getCmp("consultType4").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                                 case 5:
                                                     Ext.getCmp("consultType5").setValue(true);
                                                     ischecked = 1;
                                                     return false;

                                             }

                                         }
                                     });
                                 }
                             }
                         }
                     }
                 },
                 {
                     boxLabel: '發票及保修', id: "consultType4", inputValue: '4', checked: false,
                     listeners: {
                         change: function (radio, newValue, oldValue) {
                             if (newValue) {
                                 if (ischecked != 0) {
                                     Ext.MessageBox.confirm(CONFIRM, "更改類型會向相關負責人發送郵件，確認更改？", function (btn) {
                                         if (btn == "yes") {
                                             var zixunType = Ext.getCmp("zixunType").getValue().zixunType;
                                             Ext.Ajax.request({
                                                 url: '/ProductConsult/SendMailByGroup',
                                                 method: 'post',
                                                 params: {
                                                     zixunType: zixunType,
                                                     consult_id: row.data.consult_id,
                                                     user_name: row.data.user_name,
                                                     user_email: row.data.user_email,
                                                     parameterName: row.data.parameterName,
                                                     consult_info: row.data.consult_info,
                                                     create_date: row.data.create_date,
                                                     ckShopClass: row.data.prod_classify == 10 ? "ckShopClass1" : "ckShopClass2"
                                                 },
                                                 success: function (form, action) {
                                                     ProductConsultStore.load();
                                                     ischecked = 0;
                                                     Ext.Msg.alert("提示信息", "操作成功！");
                                                     editWin.close();
                                                 },
                                                 failure: function () {
                                                     Ext.Msg.alert(INFORMATION, FAILURE);
                                                 }
                                             });
                                         }
                                         else {
                                             ischecked = 0;
                                             var zTemp = row.data.consult_type;
                                             switch (zTemp) {
                                                 case 1:
                                                     Ext.getCmp("consultType1").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                                 case 2:
                                                     Ext.getCmp("consultType2").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                                 case 3:
                                                     Ext.getCmp("consultType3").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                                 case 4:
                                                     Ext.getCmp("consultType4").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                                 case 5:
                                                     Ext.getCmp("consultType5").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                             }

                                         }
                                     });
                                 }
                             }
                         }
                     }
                 },
                 {
                     boxLabel: '促銷及贈品', id: "consultType5", inputValue: '5', checked: false,
                     listeners: {
                         change: function (radio, newValue, oldValue) {
                             if (newValue) {
                                 if (ischecked != 0) {
                                     Ext.MessageBox.confirm(CONFIRM, "更改類型會向相關負責人發送郵件，確認更改？", function (btn) {
                                         if (btn == "yes") {
                                             var zixunType = Ext.getCmp("zixunType").getValue().zixunType;
                                             Ext.Ajax.request({
                                                 url: '/ProductConsult/SendMailByGroup',
                                                 method: 'post',
                                                 params: {
                                                     zixunType: zixunType,
                                                     consult_id: row.data.consult_id,
                                                     user_name: row.data.user_name,
                                                     user_email: row.data.user_email,
                                                     parameterName: row.data.parameterName,
                                                     consult_info: row.data.consult_info,
                                                     create_date: row.data.create_date,
                                                     ckShopClass: row.data.prod_classify == 10 ? "ckShopClass1" : "ckShopClass2"
                                                 },
                                                 success: function (form, action) {
                                                     ProductConsultStore.load();
                                                     ischecked = 0;
                                                     Ext.Msg.alert("提示信息", "操作成功！");
                                                     editWin.close();
                                                 },
                                                 failure: function () {
                                                     Ext.Msg.alert(INFORMATION, FAILURE);
                                                 }
                                             });
                                         }
                                         else {
                                             ischecked = 0;
                                             var zTemp = row.data.consult_type;
                                             switch (zTemp) {
                                                 case 1:
                                                     Ext.getCmp("consultType1").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                                 case 2:
                                                     Ext.getCmp("consultType2").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                                 case 3:
                                                     Ext.getCmp("consultType3").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                                 case 4:
                                                     Ext.getCmp("consultType4").setValue(true);
                                                     ischecked = 1;
                                                     return false;
                                                 case 5:
                                                     Ext.getCmp("consultType5").setValue(true);
                                                     ischecked = 1;
                                                     return false;

                                             }

                                         }
                                     });
                                 }
                             }



                         }
                     }
                 }
             ]


         },
          {
              xtype: 'displayfield',
              name: 'consult_info',
              id: 'consult_info',
              submitValue: true,
              fieldLabel: '諮詢內容'
          },
          {
              xtype: 'displayfield',
              fieldLabel: '回覆諮詢',
          },
          {
              xtype: 'radiogroup',
              hidden: false,
              id: 'is_sendEmail',
              name: 'is_sendEmail',
              fieldLabel: '是否發郵件',
              colName: 'is_sendEmail',
              columns: 2,
              margin: '0 0 0 105',
              vertical: true,
              items: [
                    {
                        boxLabel: '是',
                        name: 'is_sendEmail',
                        id: 'is_sendEmail_yes',
                        inputValue: "1",
                        checked: true,

                    },
                    {
                        boxLabel: '否',
                        name: 'is_sendEmail',
                        id: 'is_sendEmail_no',
                        inputValue: "0",
                        checked: false,

                    }
                    ]
          },
          {
              xtype: 'radiogroup',
              hidden: false,
              id: 'status',
              name: 'status',
              fieldLabel: '是否顯示諮詢',
              colName: 'status',
              columns: 2,
              margin: '0 0 0 105',
              vertical: true,
              items: [
                    {
                        boxLabel: '是',
                        name: 'status',
                        id: 'status_yes',
                        inputValue: "1",
                        checked: true,

                    },
                    {
                        boxLabel: '否',
                        name: 'status',
                        id: 'status_no',
                        inputValue: "0",
                        checked: false,

                    }
              ]
          },
          {
              xtype: 'radiogroup',
              hidden: false,
              id: 'answer_status',
              name: 'answer_status',
              fieldLabel: '處理狀況',
              colName: 'status',
              columns: 2,
              margin: '0 0 0 105',
              vertical: true,
              items: [
                    {
                        boxLabel: '回覆',
                        name: 'answer_status',
                        id: 'answer_status_yes',
                        inputValue: "3",
                        checked: true,

                    },
                    {
                        boxLabel: '推遲回覆',
                        name: 'answer_status',
                        id: 'answer_status_delay',
                        inputValue: "2",
                        checked: false,

                    }
              ],
              listeners: {
                          change: function () {
                              var delayreason = Ext.getCmp("answer_status").getValue().answer_status;
                              if (delayreason == 2) {
                                  Ext.getCmp("consult_answer").allowBlank = true;
                                  Ext.getCmp("consult_answer").setValue(row.data.consult_answer);
                                  Ext.getCmp("consult_answer").hide();
                                  Ext.getCmp("consult_answer").isValid();
                                  Ext.getCmp("delay_reason").show();
                                  Ext.getCmp("delay_reason").allowBlank = false;
                                  //Ext.getCmp("delay_reason").isValid();
                                  Ext.getCmp("is_sendEmail_yes").setValue(false);
                                  Ext.getCmp("is_sendEmail_no").setValue(true);
                                  Ext.getCmp("is_sendEmail_yes").disabled = true;
                                  Ext.getCmp("is_sendEmail_no").disabled = true;
                              }
                              else if (delayreason == 1 || delayreason == 3) {

                                  Ext.getCmp("delay_reason").allowBlank = true;
                                  Ext.getCmp("delay_reason").setValue(row.data.delay_reason);
                                  Ext.getCmp("delay_reason").hide();
                                  Ext.getCmp("delay_reason").isValid();
                                  Ext.getCmp("consult_answer").allowBlank = false;
                                  Ext.getCmp("consult_answer").show();
                                  //Ext.getCmp("is_sendEmail_yes").setValue(true);
                                  //Ext.getCmp("is_sendEmail_no").setValue(true);
                                  Ext.getCmp("is_sendEmail_yes").disabled = false;
                                  Ext.getCmp("is_sendEmail_no").disabled = false;
                              }
                          }
                      }
          },
          //{
          //    xtype: 'radiogroup',
          //    hidden: false,
          //    id: 'answer_status',
          //    name: 'answer_status',
          //    fieldLabel: '回覆諮詢',
          //    colName: 'answer_status',
          //    columns: 3,
          //    margin: '0 0 0 105',
          //    vertical: true,
          //    items: [
          //          {
          //              boxLabel: '是否發郵件',
          //              name: 'is_sendEmail',
          //              id: 'is_sendEmail',
          //              inputValue: "1",
          //              checked: true,

          //          },
          //          {
          //              boxLabel: '是否顯示諮詢',
          //              name: 'status',
          //              id: 'status',
          //              inputValue: "1",
          //              checked: true,

          //          },
          //          {
          //              boxLabel: '回覆',
          //              name: 'answer_status',
          //              id: 'answer_no',
          //              inputValue: "1",
          //              checked: true,

          //          },
          //          {
          //              boxLabel: '推遲回覆',
          //              name: 'answer_status',
          //              id: 'answer_delay',
          //              inputValue: "2",
          //              //checked: true
          //          },
          //          {
          //              boxLabel: '回覆不顯示',
          //              name: 'answer_status',
          //              id: 'answer_yes',
          //              inputValue: "3",
          //              //checked: true
          //          },
          //          {
          //              boxLabel: '已處理 (不會寄出通知信)',
          //              name: 'answer_status',
          //              id: 'answer_finish',
          //              inputValue: "4",
          //              //checked: true
          //          },
          //    ],
          //    listeners: {
          //        change: function () {
          //            var delayreason = Ext.getCmp("answer_status").getValue().answer_status;
          //            if (delayreason == 2) {
          //                Ext.getCmp("consult_answer").allowBlank = true;
          //                Ext.getCmp("consult_answer").setValue(row.data.consult_answer);
          //                Ext.getCmp("consult_answer").hide();
          //                Ext.getCmp("consult_answer").isValid();
          //                Ext.getCmp("delay_reason").show();
          //                Ext.getCmp("delay_reason").allowBlank = false;
          //                //Ext.getCmp("delay_reason").isValid();
          //            }
          //            else if (delayreason == 1 || delayreason == 3) {

          //                Ext.getCmp("delay_reason").allowBlank = true;
          //                Ext.getCmp("delay_reason").setValue(row.data.delay_reason);
          //                Ext.getCmp("delay_reason").hide();
          //                Ext.getCmp("delay_reason").isValid();
          //                Ext.getCmp("consult_answer").allowBlank = false;
          //                Ext.getCmp("consult_answer").show();
          //            }
          //            else if (delayreason == 4)
          //            {
          //                Ext.getCmp("delay_reason").allowBlank = true;
          //                Ext.getCmp("delay_reason").setValue(row.data.delay_reason);
          //                Ext.getCmp("delay_reason").hide();
          //                //Ext.getCmp("delay_reason").isValid();
          //                Ext.getCmp("consult_answer").allowBlank = true;
          //                Ext.getCmp("consult_answer").hide();
          //            }
          //        }
          //    }

          //},
          {
              xtype: 'textareafield',
              name: 'delay_reason',
              id: 'delay_reason',
              submitValue: true,
              //allowBlank: false,
              hidden: true,
              fieldLabel: '推遲原因（100字以內）'
          },
        {
            xtype: 'textareafield',
            name: 'consult_answer',
            id: 'consult_answer',
            allowBlank: false,
            submitValue: true,
            fieldLabel: '回覆內容（200字以內）'
        }
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: '保存',
            handler: function () {
                var form = this.up('form').getForm();
                var checkL = Ext.getCmp('consult_answer').getValue();
                checkL = checkL.replace(/[ ]/g, "");
                var checkR = Ext.getCmp('delay_reason').getValue();
                checkR = checkR.replace(/[ ]/g, "");
                var ss = Ext.htmlEncode(Ext.getCmp('answer_status').getValue().answer_status);
                ////
                //alert(Ext.htmlEncode(Ext.getCmp('answer_status').getValue().answer_status));
                //alert(Ext.htmlEncode(Ext.getCmp('is_sendEmail').getValue().is_sendEmail));
                //return;

                ////
                if (checkL.length <= 0 && ss == 3)
                {
                    Ext.Msg.alert("提示信息", "回覆內容不能為空！");
                    return;
                }
                if (checkR.length <= 0 && ss == 2) {
                    Ext.Msg.alert("提示信息", "推遲原因不能為空！");
                    return;
                }
                if (checkR.length > 100 && ss == 2) {
                    Ext.Msg.alert("提示信息", "推遲原因已超出100字！");
                    return;
                }
                if (checkL.length > 200 && ss == 3)
                {
                    Ext.Msg.alert("提示信息", "回覆內容已超出200字！");
                    return;
                }
                var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "loding..." });
                myMask.show();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            consult_id: Ext.htmlEncode(Ext.getCmp('consult_id').getValue()),
                            consult_info: Ext.htmlEncode(Ext.getCmp('consult_info').getValue()),
                            consult_answer: Ext.getCmp('consult_answer').getValue(),
                            user_name: Ext.htmlEncode(Ext.getCmp('user_name').getValue()),
                            user_email: Ext.htmlEncode(Ext.getCmp('user_email').getValue()),
                            product_name: Ext.htmlEncode(Ext.getCmp('product_name').getValue()),
                            consult_url: Ext.htmlEncode(Ext.getCmp('consult_url').getValue()),
                            product_url: Ext.htmlEncode(Ext.getCmp('product_url').getValue()),
                            product_id: Ext.htmlEncode(Ext.getCmp('product_id').getValue()),
                            is_sendEmail: Ext.htmlEncode(Ext.getCmp('is_sendEmail').getValue().is_sendEmail),
                            status: Ext.getCmp('status').getValue().status,
                            item_id: Ext.htmlEncode(Ext.getCmp('item_id').getValue()),
                            spec_id: Ext.htmlEncode(Ext.getCmp('spec_id').getValue()),
                            answer_status: Ext.htmlEncode(Ext.getCmp('answer_status').getValue().answer_status),
                            delay_reason: Ext.htmlEncode(Ext.getCmp('delay_reason').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, "保存成功! ");
                                ProductConsultStore.load();
                                ischecked = 0;
                                editWin.close();
                                myMask.hide();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                ProductConsultStore.load();
                                ischecked = 0;
                                editWin.close();
                                myMask.hide();
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, "保存失敗! ");
                            ProductConsultStore.load();
                            ischecked = 0;
                            editWin.close();
                            myMask.hide();
                        }
                    });
                }
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: '回覆諮詢',
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 600,
        height: document.documentElement.clientHeight * 260 / 783,
        y: 200,
        height: 480,
        layout: 'fit',
        items: [editFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        autoScroll: true,
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
                         ischecked = 0;
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
                if (row) {
                    editFrm.getForm().loadRecord(row);
                    if (row.data.consult_type == 1) {
                        Ext.getCmp("consultType1").setValue(true);
                        ischecked = 1;
                    }
                    if (row.data.consult_type == 2) {
                        Ext.getCmp("consultType2").setValue(true);
                        ischecked = 1;
                    }
                    if (row.data.consult_type == 3) {
                        Ext.getCmp("consultType3").setValue(true);
                        ischecked = 1;
                    }
                    if (row.data.consult_type == 4) {
                        Ext.getCmp("consultType4").setValue(true);
                        ischecked = 1;
                    }
                    if (row.data.consult_type == 5) {
                        Ext.getCmp("consultType5").setValue(true);
                        ischecked = 1;
                    }
                    if (row.data.status == 1)
                    {
                        Ext.getCmp("status_yes").setValue(true);
                        Ext.getCmp("status_no").setValue(false);
                    }
                    if (row.data.status == 0)
                    {
                        Ext.getCmp("status_yes").setValue(false);
                        Ext.getCmp("status_no").setValue(true);
                    }
                    if (row.data.is_sendEmail == 1)
                    {
                        Ext.getCmp("is_sendEmail_yes").setValue(true);
                        Ext.getCmp("is_sendEmail_no").setValue(false);
                    }
                    if (row.data.is_sendEmail == 0)
                    {
                        Ext.getCmp("is_sendEmail_yes").setValue(false);
                        Ext.getCmp("is_sendEmail_no").setValue(true);
                    }
                    if (row.data.answer_status == 2)
                    {
                        Ext.getCmp("answer_status_yes").setValue(false);
                        Ext.getCmp("answer_status_delay").setValue(true);

                        Ext.getCmp("is_sendEmail_yes").setValue(false);
                        Ext.getCmp("is_sendEmail_no").setValue(true);
                        Ext.getCmp("is_sendEmail_yes").disabled = true;
                        Ext.getCmp("is_sendEmail_no").disabled = true;
                    }
                    if (row.data.answer_status == 3)
                    {                
                        Ext.getCmp("answer_status_yes").setValue(true);
                        Ext.getCmp("answer_status_delay").setValue(false);
                    }
                    //if (row.data.answer_status == 1) {
                    //    Ext.getCmp("answer_no").setValue(true);
                    //    Ext.getCmp("delay_reason").hide();
                    //    Ext.getCmp("delay_reason").allowBlank = true;
                    //}
                    //if (row.data.answer_status == 2) {
                    //    Ext.getCmp("answer_delay").setValue(true);
                    //    Ext.getCmp("answer_no").setValue(false);
                    //    Ext.getCmp("delay_reason").show();
                    //    Ext.getCmp("consult_answer").hide();
                    //}
                    //if (row.data.answer_status == 3) {
                    //    Ext.getCmp("answer_delay").disabled = true;
                    //    Ext.getCmp("answer_delay").readOnly = true;
                    //    if (row.data.status == 0) {
                    //        Ext.getCmp("answer_yes").setValue(true);
                    //        Ext.getCmp("answer_no").setValue(false);
                    //        Ext.getCmp("answer_finish").setValue(false);
                    //    }
                    //}
                    //if (row.data.answer_status == 4)
                    //{
                    //    Ext.getCmp("answer_delay").disabled = true;
                    //    Ext.getCmp("answer_delay").readOnly = true;
                    //    //if (row.data.status == 0)
                    //    {
                    //        Ext.getCmp("answer_yes").setValue(false);
                    //        Ext.getCmp("answer_no").setValue(false);
                    //        Ext.getCmp("answer_finish").setValue(true);
                    //    }
                    //}

                }

            }
        }
    });
    //editWin.show();
}