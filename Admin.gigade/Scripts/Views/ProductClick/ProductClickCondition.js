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
//品牌Model
Ext.define("gigade.Brand", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "Brand_Id", type: "string" },
        { name: "Brand_Name", type: "string" }]
});

//品牌store
var brandStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Brand',
    autoLoad: true,
    //filterOnLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Product/GetVendorBrand",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
//商品館別Store
var ProdClassfyStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '全部', "value": "0" },
        { "txt": '食品館', "value": "10" },
        { "txt": '用品館', "value": "20" }]
});
Ext.define("gigade.paraModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});
var statusStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=product_status',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
//查询
Query = function () {
    //ProductClickStore.removeAll();

    var type = Ext.getCmp('type').getValue();
    switch (type.type) {
        case "b":
            Ext.getCmp('year').show();
            Ext.getCmp('month').show();
            Ext.getCmp('day').show();
            break;
        case "y":
            Ext.getCmp('year').show();
            Ext.getCmp('month').hide();
            Ext.getCmp('day').hide();
            break;
        case "m":
            Ext.getCmp('year').hide();
            Ext.getCmp('month').show();
            Ext.getCmp('day').hide();
            break;
        case "d":
            Ext.getCmp('year').hide();
            Ext.getCmp('month').hide();
            Ext.getCmp('day').show();
            break;

    }
    Ext.getCmp("gdProductClick").store.loadPage(1, {
        params: {
            product_status: Ext.getCmp('product_status').getValue(),
            brand_id: Ext.getCmp('brand_id').getValue(),
            prod_classify: Ext.getCmp('prod_classify').getValue(),
            product_id: Ext.getCmp('product_id').getValue(),
            type: Ext.getCmp('type').getValue(),
            startdate: Ext.getCmp('startdate').getValue(),
            enddate: Ext.getCmp('enddate').getValue()
        }
    });
}
function TheMonthFirstDay() {
    var times;
    times = new Date();
    return new Date(times.getFullYear(), times.getMonth(), 1);
}
function Tomorrow(month) {
    var d;
    d = new Date();                             // 创建 Date 对象。
    //d.setDate(d.getDate() + days);
    d.setMonth(d.getMonth() - 1);
    return d;
}
var frm = Ext.create('Ext.form.Panel', {
    id: 'frm',
    layout: 'anchor',
    flex: 1.8,
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
                     xtype: 'combobox', //status
                     fieldLabel: '商品狀態',
                     editable: false,
                     id: 'product_status',
                     margin: '0 5px',
                     store: statusStore,
                     displayField: 'parameterName',
                     valueField: 'parameterCode',
                     typeAhead: true,
                     forceSelection: false,
                     allowBlank: true,
                     emptyText: '請選擇'
                     //,
                     //listeners: {
                     //    "select": function (combo, record)
                     //    {
                     //        var z = Ext.getCmp("product_status");
                     //        if (z.getValue() == "0" || z.getValue() == "20")
                     //        {//新建立商品和供應商新建商品
                     //            Ext.Msg.alert(INFORMATION, "不存在該狀態的商品");
                     //            z.setValue("");
                     //        }
                     //    }
                     //}
                 },
                {//品牌
                    xtype: 'combobox',
                    fieldLabel: '品牌',
                    labelWidth: 40,
                    defaultListConfig: {              //取消loading的Mask
                        loadMask: false,
                        loadingHeight: 70,
                        minWidth: 70,
                        maxHeight: 300,
                        shadow: "sides"
                    },
                    id: 'brand_id',
                    name: 'brand_id',
                    colName: 'brand_id',
                    editable: true,
                    store: brandStore,
                    queryMode: 'local',
                    displayField: 'Brand_Name',
                    valueField: 'Brand_Id',
                    typeAhead: true,
                    triggerAction: 'all',
                    forceSelection: true,
                    emptyText: '請選擇'
                },
                {
                    xtype: 'combobox',
                    id: 'prod_classify',
                    name: 'prod_classify',
                    margin: '0 5px',
                    fieldLabel: '商品館別',
                    queryMode: 'local',
                    editable: false,
                    store: ProdClassfyStore,
                    displayField: 'txt',
                    valueField: 'value',
                    value: 0
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
                    margin: '0 0 0 5',
                    fieldLabel: '查詢日期'
                },
                {
                    xtype: 'datefield',
                    id: 'startdate',
                    name: 'startdate',
                    margin: '0 5 0 0',
                    editable: false,
                    value: Tomorrow(1),
                    format: 'Y/m/d',
                    vtype: 'daterange',
                    endDateField: 'enddate',
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("startdate");
                            var end = Ext.getCmp("enddate");
                            if (end.getValue() == null) {
                                end.setValue(setNextMonth(start.getValue(), 1));
                            }
                            else if (end.getValue() < start.getValue()) {
                                Ext.Msg.alert(INFORMATION, DATA_TIP);
                                start.setValue(setNextMonth(end.getValue(), -1));
                            }
                            else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                               // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                end.setValue(setNextMonth(start.getValue(), 1));
                            }
                        },
                        specialkey: function (field, e) {
                            if (e.getKey() == e.ENTER) {
                                Query();
                            }
                        }
                    }

                },

                {
                    xtype: 'displayfield',
                    value: '~'
                },
                {
                    xtype: 'datefield',
                    id: 'enddate',
                    name: 'enddate',
                    margin: '0 5px',
                    editable: false,
                    value: new Date(),
                    format: 'Y/m/d',
                    vtype: 'daterange',
                    startDateField: 'startdate',
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("startdate");
                            var end = Ext.getCmp("enddate");
                            if (start.getValue() != "" && start.getValue() != null) {
                                if (end.getValue() < start.getValue()) {
                                    Ext.Msg.alert(INFORMATION, DATA_TIP);
                                    end.setValue(setNextMonth(start.getValue(), 1));
                                }
                                else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                   // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                    start.setValue(setNextMonth(end.getValue(), -1));
                                }
                            }
                            else {
                                start.setValue(setNextMonth(end.getValue(), -1));
                            }
                        },
                        specialkey: function (field, e) {
                            if (e.getKey() == e.ENTER) {
                                Query();
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
            items: [
                   {
                       xtype: 'textfield',
                       id: 'product_id',
                       name: 'product_id',
                       margin: '0 5px',
                       fieldLabel: '商品編號'
                   },
                   {
                       xtype: 'radiogroup',
                       hidden: false,
                       id: 'type',
                       name: 'type',
                       fieldLabel: '統計類型',
                       width: 400,
                       margin: '0 5px',
                       columns: 4,
                       vertical: true,
                       items: [
                           {
                               boxLabel: '不分',
                               name: 'type',
                               id: 'bufen',
                               checked: true,
                               inputValue: "b"
                           },
                           {
                               boxLabel: '年',
                               name: 'type',
                               inputValue: "y"
                           },
                           {
                               boxLabel: '月',
                               name: 'type',
                               inputValue: "m"
                           },
                           {
                               boxLabel: '日',
                               name: 'type',
                               inputValue: "d"
                           },
                       ]
                   }]
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
                                Ext.getCmp('product_status').setValue("");
                                Ext.getCmp('prod_classify').setValue(0);
                                Ext.getCmp('brand_id').setValue("");
                                Ext.getCmp('product_id').setValue("");
                                Ext.getCmp('bufen').setValue(true);
                                Ext.getCmp('startdate').reset();//開始時間--time_start--delivery_date
                                Ext.getCmp('enddate').reset();//結束時間--time_end--delivery_date
                                Ext.getCmp('startdate').setValue(Tomorrow(1));//開始時間--time_start--delivery_date
                                Ext.getCmp('enddate').setValue(new Date());//結束時間--time_end--delivery_date
                            }
                        }
                    }
                ]
            }
    ]
});

setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}

