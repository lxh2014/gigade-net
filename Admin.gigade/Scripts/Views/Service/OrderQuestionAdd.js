var order_id;

Ext.onReady(function () {
    order_id = GetOrderId();
    questionAdd();
});

function questionAdd() {
    Ext.define('gigade.QuestionTypeModel', {
        extend: 'Ext.data.Model',
        fields: [
            { name: "ParameterCode", type: "int" },
            { name: "parameterName", type: "string" }
        ]
    });

    var QuestionTypeStore = Ext.create('Ext.data.Store', {
        autoDestroy: true,
        model: 'gigade.QuestionTypeModel',
        proxy: {
            type: 'ajax',
            url: '/Service/GetQuestionTypeList',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });
    QuestionTypeStore.load();

    var SaveFrm = Ext.create('Ext.form.Panel', {
        id: 'SaveFrm',
        frame: true,
        bodyStyle:'background:white',
        autoScroll: true,
        border: false,
        layout: 'anchor',
        url: '/Service/OrderQuestionSave',
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '訂單編號',
                id: 'orderid',
                labelWidth: 100,
                margin: '5 5 5 10',
                width: 450,
                value: order_id
            },
            {
                xtype: 'combobox',
                editable: false,
                fieldLabel: '問題分類',
                labelWidth: 100,
                margin: '5 5 5 10',
                lastQuery: '',
                width: 450,
                id: 'question_type',
                name: 'question_type',
                store: QuestionTypeStore,
                displayField: 'parameterName',
                valueField: 'ParameterCode',
                value: '0'
            },
            {
                xtype: 'textfield',
                fieldLabel: '姓名',
                margin: '5 5 5 10',
                width: 450,
                labelWidth: 100,
                id: 'name',
                name: 'name',
                allowBlank: false
            },
            {
                xtype: 'textfield',
                fieldLabel: '聯絡電話',
                margin: '5 5 5 10',
                width: 450,
                regex: /^[-+]?([0-9]\d*|0)$/,
                regexText: '格式不正確',
                labelWidth: 100,
                id: 'linkphone',
                name: 'linkphone',
                allowBlank: false
            },
            {
                xtype: 'textfield',
                fieldLabel: '電子郵件',
                margin: '5 5 5 10',
                width: 450,
                labelWidth: 100,
                vtype: 'email',
                id: 'email',
                name: 'email',
                allowBlank: false
            },
             {
                 xtype: 'fieldcontainer',
                 combineErrors: true,
                 fieldLabel: '回覆方式',
                 margin: '5 5 5 10',
                 labelWidth: 100,
                 layout: 'hbox',
                 items: [
                   {
                       xtype: 'checkboxfield',
                       boxLabel: 'E-mail',
                       name: 'reply',
                       width: 60,
                       id: 'reply1'
                   },
                   {
                       xtype: 'checkboxfield',
                       boxLabel: '簡訊 ',
                       width: 60,
                       name: 'reply',
                       id: 'reply2'
                   },
                   {
                       xtype: 'checkboxfield',
                       boxLabel: '電話',
                       width: 60,
                       name: 'reply',
                       id: 'reply3',
                       listeners: {
                           'change': function () {
                               var check = Ext.getCmp("reply3").getValue();
                               if (check) {
                                   Ext.getCmp("ddlstatus").setDisabled(false);
                               }
                               else {
                                   Ext.getCmp("ddlstatus").setDisabled(true);
                               }
                           }
                       }
                   }

                 ]
             },
         {
             xtype: 'fieldcontainer',
             combineErrors: true,
             layout: 'hbox',
             fieldLabel: "希望回覆時間",
             labelWidth: 100,
             margin: '5 5 5 10',
             submitValue: true,
             id: 'time_condition',
             items: [ {
                 xtype: 'radiogroup',
                 id: 'ddlstatus',
                 name: 'ddlstatus',
                 disabled:true,
                 colName: 'ddlstatus',
                 columns: 4,
                 layout:'hbox',
                 width: 500,
                 defaults: {
                     name: 'Status'
                 },
                 items: [
                     { boxLabel: '上午時段 : 9點 -12點', id: 'all', inputValue: '1' },
                     { boxLabel: '下午時段 : 2點 -6點', id: 'DH', inputValue: '2' },
                     { boxLabel: '不限時段', id: 'YH', inputValue: '3'}
                 ]
             }]
         },
            {
                xtype: 'textarea',
                fieldLabel: '內容',
                margin: '5 5 5 10',
                width: 450,
                labelWidth: 100,
                height: 80,
                id: 'content',
                name: 'content',
                allowBlank: false
            },
            {
                xtype: 'button',
                text: '保存',
                id: 'save',
                width: 70,
                disabled: true,
                formBind: true,
                margin: '0 0 0 390',
                handler: function () {
                    var quesType = Ext.getCmp('question_type').getValue();
                    if (quesType != "0" && quesType != "") {
                        var form = this.up('form').getForm();
                        if (form.isValid()) {
                            form.submit({
                                params: {
                                    //裡面的數據錯誤。會導致無法顯示各種數據
                                    orderid: order_id,
                                    questiontype: quesType,
                                    name: Ext.getCmp('name').getValue(),
                                    email_id: Ext.htmlEncode(Ext.getCmp('email').getValue()),
                                    linkphone: Ext.htmlEncode(Ext.getCmp("linkphone").getValue()),
                                    content: Ext.htmlEncode(Ext.getCmp('content').getValue()),
                                    reply1: Ext.htmlEncode(Ext.getCmp('reply1').getValue()),
                                    reply2: Ext.htmlEncode(Ext.getCmp('reply2').getValue()),
                                    reply3: Ext.htmlEncode(Ext.getCmp('reply3').getValue()),
                                    ddlstatus: Ext.htmlEncode(Ext.getCmp('ddlstatus').getValue().Status)
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);
                                    if (result.success) {
                                        Ext.Msg.alert(INFORMATION, SUCCESS);
                                        Ext.Msg.confirm(INFORMATION, "新增成功!是否關閉此頁面?", function (btn) {
                                            if (btn == "yes") {
                                        TransToOrder();
                                    }
                                            else {
                                                form.reset();
                                            }
                                        })
                                        
                                    }
                                },
                                failure: function () {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            });
                        }
                    } else {
                        alert("請選擇問題原因!");
                    }
                }
            }
        ]
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: SaveFrm,
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                this.doLayout();
            }
        }
    });
}
//獲取要顯示的付款單號
function GetOrderId() {
    return document.getElementById('OrderId').value;
}

function TransToOrder() {
    var url = '/Service/OrderQuestion';
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#addOrderQuestion');
    //var copy = panel.down('#OrderQuestion');
    if (copy) {
        copy.close();
    }
    var copy = panel.add({
        id: 'OrderQuestion',
        title: '訂單問題',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}