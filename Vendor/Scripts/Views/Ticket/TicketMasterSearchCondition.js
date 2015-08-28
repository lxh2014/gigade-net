//实现验证开始时间必须小于结束时间
Ext.apply(Ext.form.field.VTypes, {
    daterange: function (val, field) {
        var date = field.parseDate(val);

        if (!date) {
            return false;
        }
        this.dateRangeMax = null;
        this.dateRangeMin = null;
        if (field.startDateField && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax.getTime()))) {
            var start = field.up('form').down('#' + field.startDateField);
            start.setMaxValue(date);
            //start.validate();
            this.dateRangeMax = date;
        } else if (field.endDateField && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin.getTime()))) {
            var end = field.up('form').down('#' + field.endDateField);
            end.setMinValue(date);
            //end.validate();
            this.dateRangeMin = date;
        }
        /*  
         * Always return true since we're only using this vtype to set the  
         * min/max allowed values (these are tested for after the vtype test)  
         */
        return true;
    },

    daterangeText: '開始時間必須小於結束時間'
});
//查询
Query = function () {
    TicketMasterStore.removeAll();
    Ext.getCmp("gdTicketMaster").store.loadPage(1, {
        params: {
            order_status: Ext.getCmp('order_status').getValue(),//訂單狀態
            order_payment: Ext.getCmp('order_payment').getValue(),//付款方式
            order_id: Ext.getCmp('o_id').getValue(),//訂單編號
            order_name: Ext.getCmp('o_name').getValue(),//訂購人
            ticket_start: Ext.getCmp('ticket_start').getValue(),//訂單開始時間
            ticket_end: Ext.getCmp('ticket_end').getValue(),//訂單結束時間
            course_search: Ext.getCmp('course_search').getValue(),//課程編號或課程名稱
            course_start: Ext.getCmp('course_start').getValue(),//課程開始時間
            course_end: Ext.getCmp('course_end').getValue(),//課程結束時間
            bill_check: Ext.getCmp('bill_check').getValue()//是否對賬
        }
    });
}
function TheMonthFirstDay() {
    var times;
    times = new Date();
    return new Date(times.getFullYear(), times.getMonth(), 1);
}
function Tomorrow(days) {
    var d;
    d = new Date();                             // 创建 Date 对象。
    d.setDate(d.getDate() + days);
    return d;
}
var frm = Ext.create('Ext.form.Panel', {
    id: 'frm',
    layout: 'anchor',
    height: 180,
    //flex:1.5,
    border: 0,
    bodyPadding: 10,
    width: document.documentElement.clientWidth,
    items: [
        {
            xtype: 'fieldcontainer',
            combineErrors: true,
            layout: 'hbox',
            items: [
                {
                    xtype: 'combobox',
                    id: 'order_status',
                    name: 'order_status',
                    margin: '0 5px',
                    fieldLabel: '訂單狀態',
                    labelWidth: 120,
                    editable: false,
                    store: paymentType,
                    displayField: 'remark',
                    valueField: 'ParameterCode',
                    emptyText: '所有方式'
                },
                {
                    xtype: 'combobox',
                    id: 'order_payment',
                    name: 'order_payment',
                    margin: '0 5px',
                    fieldLabel: '付款方式',
                    queryMode: 'local',
                    editable: false,
                    store: paymentStore,
                    displayField: 'parameterName',
                    valueField: 'parameterCode',
                    emptyText: '所有方式'
                }
            ]
        },
        {
            xtype: 'fieldcontainer',
            combineErrors: true,
            layout: 'hbox',
            items: [
                {
                    xtype: 'textfield',
                    id: 'o_id',
                    margin: '0 5px',
                    name: 'o_id',
                    fieldLabel: '訂單編號',
                    labelWidth: 120
                },
                {
                    xtype: 'textfield',
                    id: 'o_name',
                    name: 'o_name',
                    margin: '0 5px',
                    fieldLabel: '訂購人'
                }
            ]
        },
        {
            xtype: 'fieldcontainer',
            combineErrors: true,
            layout: 'hbox',
            items: [
                {
                    xtype: 'displayfield',
                    margin: '0 5px',
                    fieldLabel: '訂單日期'
                },
                {
                    xtype: 'datefield',
                    id: 'ticket_start',
                    name: 'ticket_start',
                    margin: '0 5px',
                    editable: false,
                    value: TheMonthFirstDay(),
                    format: 'Y/m/d',
                    vtype: 'daterange',
                    endDateField: 'ticket_end'

                },
                {
                    xtype: 'displayfield',
                    value: '~'
                },
                {
                    xtype: 'datefield',
                    id: 'ticket_end',
                    name: 'ticket_end',
                    margin: '0 5px',
                    editable: false,
                    value: Tomorrow(1),
                    format: 'Y/m/d',
                    vtype: 'daterange',
                    startDateField: 'ticket_start'

                }
            ]
        },
         {
             xtype: 'fieldcontainer',
             combineErrors: true,
             layout: 'hbox',
             items: [
                    {
                        xtype: 'textfield',
                        id: 'course_search',
                        name: 'course_search',
                        margin: '0 5px',
                        fieldLabel: '課程編號/課程名稱',
                        labelWidth: 120
                    },
                    {
                        xtype: 'radiogroup',
                        hidden: false,
                        id: 'bill_check',
                        name: 'bill_check',
                        fieldLabel: '是否對賬',
                        colName: 'bill_check',
                        width: 400,
                        margin: '0 5px',
                        columns: 3,
                        vertical: true,
                        items: [
                            {
                                boxLabel: '是',
                                name: 'bill_check',
                                id: 'y',
                                checked: true,
                                inputValue: "1"
                            },
                            {
                                boxLabel: '否',
                                name: 'bill_check',
                                id: 'n',
                                inputValue: "0"
                            }]
                    }]
         },
                       {
                           xtype: 'fieldcontainer',
                           combineErrors: true,
                           layout: 'hbox',
                           items: [
                               {
                                   xtype: 'displayfield',
                                   margin: '0 5px',
                                   fieldLabel: '課程日期'
                               },
                               {
                                   xtype: 'datefield',
                                   id: 'course_start',
                                   name: 'course_start',
                                   margin: '0 5px',
                                   editable: false,
                                   value: TheMonthFirstDay(),
                                   format: 'Y/m/d',
                                   vtype: 'daterange',
                                   endDateField: 'course_end'
                               },
                               {
                                   xtype: 'displayfield',
                                   value: '~'
                               },
                               {
                                   xtype: 'datefield',
                                   id: 'course_end',
                                   name: 'course_end',
                                   margin: '0 5px',
                                   editable: false,
                                   value: Tomorrow(1),
                                   format: 'Y/m/d',
                                   vtype: 'daterange',
                                   startDateField: 'course_start'
                               }
                           ]
                       },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'button',
                        margin: '0 10 0 10',
                        iconCls: 'icon-search',
                        text: "查詢",
                        handler: Query
                    },
                    {
                        xtype: 'button',
                        text: '重置',
                        id: 'btn_reset',
                        iconCls: 'ui-icon ui-icon-reset',
                        listeners: {
                            click: function () {
                                Ext.getCmp('order_status').setValue("");
                                Ext.getCmp('order_payment').setValue("");
                                Ext.getCmp('o_id').setValue("");
                                Ext.getCmp('o_name').setValue("");
                                Ext.getCmp('course_search').setValue("");
                                Ext.getCmp('n').setValue(true);
                                Ext.getCmp('ticket_start').setValue(TheMonthFirstDay());//開始時間--time_start--delivery_date
                                Ext.getCmp('ticket_end').setValue(Tomorrow(1));//結束時間--time_end--delivery_date
                                Ext.getCmp('course_start').setValue(TheMonthFirstDay());//開始時間--time_start--delivery_date
                                Ext.getCmp('course_end').setValue(Tomorrow(1));//結束時間--time_end--delivery_date
                            }
                        }
                    }
                ]
            }
    ], listeners: {
        beforerender: function () {
            var course_id = document.getElementById("course_id").value;
            if (course_id != "") {
                Ext.getCmp('course_search').setValue(course_id);
            }
        }
    }
});

