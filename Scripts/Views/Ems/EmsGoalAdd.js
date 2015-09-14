editFunction = function (row, store) {
    Ext.define('GIGADE.EmsGoalComGoal', {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'department_code', type: 'string' },
            { name: 'department_name', type: 'string' }
        ]
    });
    var EmsGoalComStoreGoal = Ext.create("Ext.data.Store", {
        autoDestroy: true,
        model: 'GIGADE.EmsGoalComGoal',
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
        url: '/Ems/SaveEmsGoal',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },
        items: [

                        {
                            xtype: 'combobox', //網頁
                            allowBlank: false,
                            editable: false,
                            fieldLabel: "部門",
                            id: 'department_code',
                            name: 'department_code',
                            store: EmsGoalComStoreGoal,
                            displayField: 'department_name',
                            valueField: 'department_code',
                            emptyText: "請選擇..."

                        },
            {
                xtype: 'numberfield',
                fieldLabel: '年份',
                id: 'year',
                name: 'year',
                allowBlank:false,
                allowDecimals: false,
                editable: false,
                minValue: 2000,
                maxValue: 9999,
                value: parseInt ((new Date).getFullYear())
            },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '月份',
                        id: 'month',
                        name: 'month',
                        allowBlank: false,
                        allowDecimals: false,
                        editable: false,
                        minValue: 1,
                        maxValue: 12,
                        value: parseInt(((new Date).getMonth() + 1))
                    },

            {
                xtype: 'numberfield',
                fieldLabel: '目標',
                allowBlank: false,
                id: 'goal_amount',
                name: 'goal_amount',
                minValue:0
            },
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
                                                    goal_amount: Ext.htmlEncode(Ext.getCmp("goal_amount").getValue())
                                                },
                                                success: function (form, action) {
                                                    var result = Ext.decode(action.response.responseText);
                                                    if (result.success) {
                                                        if (result.msg == 0)
                                                        {
                                                            Ext.Msg.alert("提示信息", "" + Ext.getCmp("month").getValue() + "月份數據已存在不可重複添加！");
                                                        }
                                                        else if (result.msg == 1)
                                                        {
                                                            Ext.Msg.alert("提示信息", "保存成功！");
                                                            store.load();
                                                            editWin.close();
                                                        }
                                                        else if (result.msg ==3) {
                                                            Ext.Msg.alert("提示信息", "所選日期大於當前日期，請重新選擇");
                                                        }
                                                    }
                                                    else {
                                                        alert(result.msg);
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
        title: "新增業績目標",
        id: 'editWin',
        iconCls:"icon-user-add",
        width: 350,
        height: 220,
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
                 Ext.MessageBox.confirm("提示信息","是否關閉窗口", function (btn) {
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

