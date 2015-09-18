editSiteStatisticsFunction = function (row, store)
{
    var sscodeStore = Ext.create("Ext.data.Store", {
        model: 'gigade.paraModel',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: '/Parameter/QueryPara?paraType=ss_code',
            noCache: false,
            getMethod: function () { return 'get'; },
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'items'
            }
        }
    });
    var editSiteStatisticsFrm = Ext.create('Ext.form.Panel', {
        id: 'editSiteStatisticsFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 80,
        url: '/SiteManager/SiteStatisticsEdit',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
               {
                   xtype: 'displayfield',
                   fieldLabel: '流水號',
                   id: 'ss_id',
                   name: 'ss_id',
                   hidden: true,
                   submitValue: true
               },
               {
                   xtype: 'numberfield',
                   fieldLabel: '曝光數',
                   id: 'ss_show_num',
                   name: 'ss_show_num',
                   allowBlank: false,
                   allowDecimals: false,
                   minValue: 0,
                   value: 0,
                   submitValue: true
               },
               {
                   xtype: 'numberfield',
                   fieldLabel: '點擊',
                   id: 'ss_click_num',
                   name: 'ss_click_num',
                   allowBlank: false,
                   allowDecimals: false,
                   minValue: 0,
                   value: 0,
                   submitValue: true
               },
               {
                   xtype: "numberfield",
                   fieldLabel: '點閱率',
                   id: 'ss_click_through',
                   name: 'ss_click_through',
                   allowDecimals: true,
                   decimalPrecision: 2,
                   allowBlank: false,
                   submitValue: true,
                   minValue: 0.00,
                   value: 0.00,
                   maxValue:100
               },
               {
                   xtype: "numberfield",
                   fieldLabel: '費用',
                   id: 'ss_cost',
                   name: 'ss_cost',
                   allowDecimals: true,
                   decimalPrecision: 2,
                   allowBlank: false,
                   submitValue: true,
                   minValue: 0.00,
                   value: 0.00
               },
                {
                   xtype: 'numberfield',
                   fieldLabel: '新會員數',
                   name: 'ss_newuser_number',
                   id: 'ss_newuser_number',
                   allowDecimals: false,
                   decimalPrecision: 2,
                   allowBlank: false,
                   submitValue: true,
                   minValue: 0.00,
                   value: 0.00
               },
              {
                  xtype: 'numberfield',
                  fieldLabel: '實際轉換',
                  id: 'ss_converted_newuser',
                  name: 'ss_converted_newuser',
                  allowBlank: false,
                  allowDecimals: false,
                  minValue: 0,
                  value: 0,
                  submitValue: true
              },
              {
                  xtype: 'numberfield',
                  fieldLabel: '訂單金額',
                  name: 'ss_sum_order_amount',
                  id: 'ss_sum_order_amount',
                  allowDecimals: false,
                  allowBlank: false,
                  submitValue: true,
                  minValue: 0.00,
                  value: 0.00
              },
               {
                   xtype: 'combobox', //status
                   fieldLabel: '廠家代碼',
                   editable: false,
                   id: 'ss_code',
                   defaultListConfig: {              //取消loading的Mask
                       loadMask: false,
                       loadingHeight: 70,
                       minWidth: 70,
                       maxHeight: 300,
                       shadow: "sides"
                   },
                   store: sscodeStore,
                   displayField: 'parameterCode',
                   valueField: 'parameterCode',
                   typeAhead: true,
                   queryMode:'local',
                   allowBlank: true,
                   value: 'YAHOO'
               },
               {
                   xtype: "datefield",
                   fieldLabel: '日期',
                   id: 'ss_date',
                   name: 'ss_date',
                   format: 'Y-m-d',
                   editable:false,
                   allowBlank: false,
                   submitValue: true,
                   value: new Date(),
                   listeners: {
                       'select': function () {
                           var date = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('ss_date').getValue()), 'Y-m-d'));
                            var today=( Ext.htmlEncode(Ext.Date.format(new Date(), 'Y-m-d')));
                            if (date > today)
                            {
                                Ext.Msg.alert("提示信息","當前選擇日期大於今日,請重新選擇");
                                Ext.getCmp('ss_date').setValue(new Date());
                            }
                       }
                   }
               }
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: SAVE,
            handler: function ()
            {
                var form = this.up('form').getForm();
                var issubmit = true;
                var date = Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('ss_date').getValue()), 'Y-m-d'));
                var today = (Ext.htmlEncode(Ext.Date.format(new Date(), 'Y-m-d')));
                if (date > today) {
                    Ext.Msg.alert("提示信息", "當前選擇日期大於今日,請重新選擇");
                    Ext.getCmp('ss_date').setValue(new Date());
                    return;
                }
                if (form.isValid())
                {
                    Ext.Ajax.request({
                        url: "/SiteManager/GetSiteStatisticsList",
                        method: 'post',
                        async: false,
                        params: {
                            //ss_id: row == null ? 0 : Ext.getCmp('ss_id').getValue(),
                            ss_code: Ext.getCmp('ss_code').getValue(),
                            ss_date: Ext.getCmp('ss_date').getValue(),
                            ispage: "false"
                        },
                        success: function (form, action)
                        {
                            var result = Ext.decode(form.responseText);
                            if (result.success)
                            {
                                if (row == null)//新增
                                {
                                    if (result.data.length > 0)
                                    {
                                        issubmit = false;
                                        Ext.Msg.alert(INFORMATION, "相同的廠商和時間不能重複添加!");
                                        Ext.getCmp('ss_date').setValue("");

                                    }
                                }
                                else//編輯
                                {
                                    if (result.data.length == 1)
                                    {
                                        if (result.data[0].ss_id != row.data.ss_id)
                                        {
                                            issubmit = false;
                                            Ext.Msg.alert(INFORMATION, "相同的廠商和時間不能重複添加!");
                                            Ext.getCmp('ss_date').setValue("");
                                        }
                                      
                                    }
                                    if (result.data.length > 1)
                                    {
                                        issubmit = false;
                                        Ext.Msg.alert(INFORMATION, "相同的廠商和時間不能重複添加!");
                                        Ext.getCmp('ss_date').setValue("");
                                    }
                                }
                            }

                        }
                    });  
                    if (issubmit) 
                    {
                        form.submit({
                            params: {
                                ss_id: Ext.htmlEncode(Ext.getCmp('ss_id').getValue()),
                                ss_show_num: Ext.htmlEncode(Ext.getCmp('ss_show_num').getValue()),
                                ss_click_num: Ext.htmlEncode(Ext.getCmp('ss_click_num').getValue()),
                                //ss_click_through: Ext.Date.format(new Date(Ext.getCmp('paperStart').getValue()), 'Y-m-d H:i:s'),
                                ss_click_through: Ext.htmlEncode(Ext.getCmp('ss_click_through').getValue()),
                                ss_cost: Ext.htmlEncode(Ext.getCmp('ss_cost').getValue()),
                                ss_newuser_number: Ext.getCmp('ss_newuser_number').getValue(),
                                ss_sum_order_amount: Ext.htmlEncode(Ext.getCmp('ss_sum_order_amount').getValue()),
                                ss_converted_newuser: Ext.htmlEncode(Ext.getCmp('ss_converted_newuser').getValue()),
                                ss_date: Ext.htmlEncode(Ext.getCmp('ss_date').getValue()),
                                ss_code: Ext.htmlEncode(Ext.getCmp('ss_code').getValue())
                            },
                            success: function (form, action)
                            {
                                var result = Ext.JSON.decode(action.response.responseText);
                                if (result.success)
                                {
                                    Ext.Msg.alert(INFORMATION, result.msg);
                                    store.load();
                                    editSiteStatisticsWin.close();

                                }

                            },
                            failure: function (form, action)
                            {
                                var result = Ext.JSON.decode(action.response.responseText);
                                Ext.Msg.alert(INFORMATION, result.msg);
                                store.load();
                                editSiteStatisticsWin.close();

                            }
                        });
                    }
                }
            }
        }]
    });
    var editSiteStatisticsWin = Ext.create('Ext.window.Window', {
        iconCls: 'icon-user-edit',
        id: 'editSiteStatisticsWin',
        width: 500,
        layout: 'fit',
        items: [editSiteStatisticsFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        title: row == null ? '新增' : '編輯',
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: '是否關閉',
             handler: function (event, toolEl, panel)
             {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn)
                 {
                     if (btn == "yes")
                     {
                         Ext.getCmp('editSiteStatisticsWin').destroy();
                     }
                     else
                     {
                         return false;
                     }
                 });
             }
         }
        ],
        listeners: {
            'show': function ()
            {
                if (row != null)
                {
                    editSiteStatisticsFrm.getForm().loadRecord(row);
                    Ext.getCmp('ss_date').setValue(row.data.ss_date.toString().substr(0, 10));
                }

            }
        }
    });
    editSiteStatisticsWin.show();
}

