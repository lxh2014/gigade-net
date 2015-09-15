

addFunction = function (row, store) {

    Ext.define('GIGADE.UserInfo', {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'file_name', type: 'string' },
            { name: 'user_email', type: 'string' },
            { name: 'user_company_id', type: 'string' },
            { name: 'user_mobile', type: 'string' },
            { name: 'user_name', type: 'string' }

        ]
    })
    var UserInfoStore = Ext.create('Ext.data.Store', {
        autoLoad: false,
        model: 'GIGADE.UserInfo',
        proxy: {
            type: 'ajax',
            url: '/Service/GetUserInfo',
            actionMethods: 'post',
            render: {
                type: 'json'
            }
        }
    })

    var SaveFrm = Ext.create('Ext.form.Panel', {
        id: 'SaveFrm',
        frame: true,
        autoScroll: true,
        defaultType: 'textfield',
        layout: 'anchor',
        labelWidth: 45,
        url: '/Service/Save',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'combobox',
                id: 'type_id',
                name: 'type_id',
                fieldLabel: '類型',
                labelWidth: 75,
                queryMode: 'local',
                editable: false,
                store: QuestionStore,
                displayField: 'txt',
                valueField: 'value',
                value: 0
            },
            {
                xtype: 'combobox',
                fieldLabel: '聯絡人',
                id: 'linkpeople',//定义上下文查找ID
                name: 'linkpeople',//表单提交ID
                displayField: 'file_name',
                labelWidth: 75,
                store: UserInfoStore,
                allowBlank: false,
                // matchFieldWidth: true,
                // forceSelection: true,
                triggerAction: 'all',
                queryMode: 'local',
                typeAhead: true,
                valueField: 'file_name',
                //selectFlag: true,//很关键，由于文本数据改变事件执行的优先级别比较高，需要根据一个标记来判断是否需要输入数据
                listeners: {
                    //很关键：“beforeselect”选择下拉列表数据之前触发，设置一个标记，选择下拉列表时禁用change事件（在选择下拉列表同时会触发change事件导致重复查询 2013-12-19）
                    //'beforeselect': function () {
                    //    var myc = Ext.getCmp('linkpeople');
                    //    myc.selectFlag = false;
                    //},
                    select: function (combo, record, o) {
                        var store = combo.store;
                        var input = Ext.getCmp('linkpeople').getValue();
                        if (store.getCount() != 0) {
                            //  var index =store.find("file_name", Ext.getCmp('linkpeople').getValue());
                            Ext.getCmp('company_id').setValue(store.getAt(store.find("file_name", input)).data.user_company_id);
                            Ext.getCmp('linkphone').setValue(store.getAt(store.find("file_name", input)).data.user_mobile);
                            Ext.getCmp('email_id').setValue(store.getAt(store.find("file_name", input)).data.user_email);
                        }
                    },
                    'beforequery': function (e) {
                        var combo = e.combo;
                        if (!e.forceAll) {
                            var input = e.query;
                            //  combo.store.removeAll();
                            input = (input != 'null' && input != null) ? input : "";
                            combo.store.load({
                                callback: function (records, operation, success) {
                                    if (combo.store.getCount() == 0) {
                                        combo.setValue(input);
                                        Ext.getCmp('company_id').setValue("");
                                        Ext.getCmp('linkphone').setValue("");
                                        Ext.getCmp('email_id').setValue("");
                                    }
                                },
                                //传入后台参数设置
                                params: {
                                    name: input
                                }
                            });
                        }
                    }
                    //,
                    ////很关键：“change”文本框数据改变触发
                    //'change': function () {
                    //    var myc = Ext.getCmp('linkpeople');

                    //    var typeIdValue = myc.getValue();

                    //    //判断不在继续输入时，不调用查询服务
                    //    if (myc.selectFlag && myc.isValid()) {


                    //        //myc.store.removeAll();
                    //        //typeIdValue = (typeIdValue != 'null' && typeIdValue != null) ? typeIdValue : "";
                    //        //myc.store.load({
                    //        //    callback: function (records, operation, success) {
                    //        //        if (myc.store.getCount() == 0) {
                    //        //            return false;
                    //        //        }
                    //        //    },
                    //        //    //传入后台参数设置
                    //        //    params: {
                    //        //        name: typeIdValue
                    //        //    }
                    //        //});
                    //    }
                    //    //在抵御一次数据的改变后（用户选择数据时文本框数据发生改变，又会触发一次change事件进行一次查询操作），将查询标记设为可用
                    //    myc.selectFlag = true;//标记
                    //}


                }
            },
            {
                xtype: 'textfield',
                fieldLabel: '公司',
                labelWidth: 75,
                id: 'company_id',
                name: 'company_id',
                allowBlank: true
            },
          {
              xtype: 'textfield',
              fieldLabel: '聯絡電話',
              labelWidth: 75,
              id: 'linkphone',
              name: 'linkphone',
              allowBlank: true
          },

         {
             xtype: 'textfield',
             fieldLabel: '電子郵件',
             labelWidth: 75,
             vtype: 'email',
             id: 'email_id',
             name: 'email_id',
             allowBlank: false
         },
         {
             xtype: 'fieldcontainer',
             combineErrors: true,
             layout: 'hbox',
             items: [
               { xtype: 'displayfield', labelWidth: 75, value: "回覆方式:" },
               {
                   xtype: 'checkboxfield',
                   boxLabel: 'E-mail',
                   margin: '0 0 0 15',
                   name: 'reply',
                   id: 'reply1'
               },
               {
                   xtype: 'checkboxfield',
                   boxLabel: '簡訊 ',
                   name: 'reply',
                   margin: '0 0 0 10',
                   id: 'reply2'
               },
               {
                   xtype: 'checkboxfield',
                   boxLabel: '電話',
                   margin: '0 0 0 10',
                   name: 'reply',
                   id: 'reply3',
                   listeners: {
                       'change': function () {
                           var check = Ext.getCmp("reply3").getValue();
                           if (check) {
                               Ext.getCmp("time_condition").setDisabled(false);
                           }
                           else {
                               Ext.getCmp("time_condition").setDisabled(true);
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
             disabled: true,
             submitValue:true,
             id:'time_condition',
             items: [
                  { xtype: 'displayfield',labelWidth:75, value: "希望回覆時間:" },
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
                    boxLabel: '不限時段 ',
                    name: 'phone',
                    id: 'phone3'
                }
             ]
         },
           {
               xtype: 'textarea',
               fieldLabel: '內容',
               height: 80,
               labelWidth: 75,
               id: 'content',
               name: 'content',
               allowBlank: false
           }
        ],
        buttons: [{
            text: "保存",
            formBind: true,
            vtype: 'submit',
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    this.disable();
                    form.submit({
                        params: {
                            //裡面的數據錯誤。會導致無法顯示各種數據
                            type_id: Ext.htmlEncode(Ext.getCmp('type_id').getValue()),
                            company_id: Ext.getCmp('company_id').getValue(),
                            linkpeople: Ext.htmlEncode(Ext.getCmp('linkpeople').getValue()),
                            email_id: Ext.htmlEncode(Ext.getCmp('email_id').getValue()),
                            reply1: Ext.getCmp('reply1').getValue(),
                            reply2: Ext.getCmp('reply2').getValue(),
                            reply3: Ext.getCmp('reply3').getValue(),
                            phone1: Ext.getCmp('phone1').getValue(),
                            phone2: Ext.getCmp('phone2').getValue(),
                            phone3: Ext.getCmp('phone3').getValue(),
                            linkphone: Ext.htmlEncode(Ext.getCmp("linkphone").getValue().Tax_Type),
                            content: Ext.htmlEncode(Ext.getCmp('content').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            if (result.success) {
                                ContactUsStore.load();
                                editWin.close();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, ERRORSHOW + result.success);
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
    var editWin = Ext.create('Ext.window.Window', {
        title: "新增客服記錄",
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 450,
        layout: 'fit',
        items: [SaveFrm],
        y: 200,
        closeAction: 'destroy',
        modal: true,
        constrain: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false
        ,
        tools: [
         {
             type: 'close',
             qtip: "關閉",
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
                SaveFrm.getForm().reset(); //如果是添加的話
            }
        }
    });
    editWin.show();
}
