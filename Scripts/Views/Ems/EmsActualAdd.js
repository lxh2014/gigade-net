editFunction = function (row, store) {
    Ext.define('GIGADE.EmsGoalComActual', {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'department_code', type: 'string' },
            { name: 'department_name', type: 'string' }
        ]
    });
    var EmsGoalComStoreActual = Ext.create("Ext.data.Store", {
        autoDestroy: true,
        model: 'GIGADE.EmsGoalComActual',
        proxy: {
            type: 'ajax',
            url: '/Ems/GetDepartmentStore',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/Ems/SaveEmsActual',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },
        items: [

                        {
                            xtype: 'combobox', //網頁
                            allowBlank: false,
                            editable: false,
                            fieldLabel: "部門",
                            id: 'department_code',
                            name: 'department_code',
                            store: EmsGoalComStoreActual,
                            displayField: 'department_name',
                            valueField: 'department_code',
                            value: '通路發展部'
                        },
            {
                xtype: 'numberfield',
                fieldLabel: '年',
                id: 'year',
                name: 'year',
                allowBlank: false,
                allowDecimals: false,
                minValue: 2000,
                maxValue: 9999,
                editable:false,
                value: parseInt((new Date).getFullYear())
            },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '月',
                        id: 'month',
                        name: 'month',
                        allowBlank: false,
                        allowDecimals: false,
                        minValue: 1,
                        maxValue: 12,
                        editable: false,
                        value: parseInt(((new Date).getMonth() + 1))
                    },
                            {
                                xtype: 'numberfield',
                                fieldLabel: '日',
                                id: 'day',
                                name: 'day',
                                allowBlank: false,
                                allowDecimals: false,
                                editable: false,
                                minValue: 1,
                                maxValue: 31,
                                value: parseInt(((new Date).getDate())-1)
                            },
            {
                xtype: 'numberfield',
                fieldLabel: '成本',
                allowBlank: false,
                id: 'cost_sum',
                name: 'cost_sum',
                allowDecimals:false,
                minValue: 0
            },
               {
                   xtype: 'numberfield',
                   fieldLabel: '訂單總數',
                   allowBlank: false,
                   id: 'order_count',
                   name: 'order_count',
                   allowDecimals: false,
                   minValue: 0
               },
                  {
                      xtype: 'numberfield',
                      fieldLabel: '累計實績',
                      allowBlank: false,
                      id: 'amount_sum',
                      name: 'amount_sum',
                      allowDecimals: false,
                      minValue: 0
                  }
        ],
        buttons: [{
            text: "保存",
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            department_code: Ext.htmlEncode(Ext.getCmp("department_code").getValue()),
                            year: Ext.htmlEncode(Ext.getCmp("year").getValue()),
                            month: Ext.htmlEncode(Ext.getCmp("month").getValue()),
                            day: Ext.htmlEncode(Ext.getCmp("day").getValue()),
                            cost_sum: Ext.htmlEncode(Ext.getCmp("cost_sum").getValue()),
                            order_count: Ext.htmlEncode(Ext.getCmp("order_count").getValue()),
                            amount_sum: Ext.htmlEncode(Ext.getCmp("amount_sum").getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                if (result.msg == 0) {
                                    Ext.Msg.alert("提示信息", "此日期下數據已存在不可重複添加！");
                                }
                                else if (result.msg == 1) {
                                    Ext.Msg.alert("提示信息", "保存成功！");
                                    store.load();
                                    editWin.close();
                                }
                                else if (result.msg == 2) {
                                    Ext.Msg.alert("提示信息", "保存失敗！");
                                    store.load();
                                    editWin.close();
                                }
                                else if (result.msg == 3) {
                                    Ext.Msg.alert("提示信息", "您選的日期有誤,請重新選擇！");
                                }
                                else if (result.msg == 4) {
                                    Ext.Msg.alert("提示信息", "您選的日期大於當前日期,請重新選擇！");
                                }
                            }
                            else{
                                Ext.Msg.alert("提示信息", "保存失敗！");
                                store.load();
                                editWin.close();
                            }
                        }
                    });
                }
            }
        }
        ]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: "新增實際業績",
        id: 'editWin',
        iconCls: "icon-user-add",
        width: 350,
        height: 300,
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
                 Ext.MessageBox.confirm("提示信息", "是否關閉窗口", function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }]
    });
    editWin.show();
}

