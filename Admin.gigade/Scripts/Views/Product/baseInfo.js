//var isCheck = true;
var columnCount_freight;
var columnCount_mode;
var PRODUCT_ID, OLD_PRODUCT_ID = '';
var isChange = false;
var SAVEPANEL, COURSEPANEL;
var productInfo;///add by wwei0216w 定義一個productInfo 用來存儲查詢到的商品信息,以便其他代碼調用
//123456git111456
//品牌Model
Ext.define("gigade.Brand", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "brand_id", type: "string" },
        { name: "brand_name", type: "string" }]
});

//品牌store
var brandStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Brand',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Brand/QueryBrand?hideOffGrade=1",
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item'
        }
    }
});

//productTypeStore add 2014/09/15
Ext.define("gigade.ProductTypeStore", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "parameterCode", type: "string" },
        { name: "parameterName", type: "string" }]
});

var productTypeStore = Ext.create('Ext.data.Store', {
    model: 'gigade.ProductTypeStore',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Parameter/QueryPara?paraType=product_type",
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//tierStore add 2015/03/26
var tierStore = Ext.create('Ext.data.Store', {
    fields: [{ name: 'schedule_Id', type: 'int' },
             { name: 'desc', type: 'string' },
             { name: 'schedule_name', type: 'string' }],
    idProperty: 'schedule_Id',
    proxy: {
        type: 'ajax',
        url: "/ProductTier/GetTiersByConditon",
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json'
            //root: 'items'
        }
    }
});

//productTypeStore add 2014/09/15
Ext.define("gigade.ProductTypeStore", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "parameterCode", type: "string" },
        { name: "parameterName", type: "string" }]
});

var productTypeStore = Ext.create('Ext.data.Store', {
    model: 'gigade.ProductTypeStore',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Parameter/QueryPara?paraType=product_type",
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//課程類型 2015/03/10
Ext.define("gigade.CourseTypeStore", {
    extend: 'Ext.data.Model',
    fields: [{ name: 'Course_Id', type: 'int' },
             { name: 'Course_Name', type: 'string' }]
});

var courseTypeStore = Ext.create('Ext.data.Store', {
    model: 'gigade.CourseTypeStore',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Course/GetCurricul?IsPage=false",
        noCache: false,
        //getMethod: function () { return 'get'; },
        actionMethods: 'get',
        reader: {
            type: 'json'
        }
    }
});

Ext.onReady(function () {
    var tomorrow = Tomorrow();
    var tomRepeat = Tomorrow();
    var tureorfalse = true;

    PRODUCT_ID = window.parent.GetProductId();
    if (PRODUCT_ID > 0) {
        tureorfalse = false;
    }
    OLD_PRODUCT_ID = window.parent.GetCopyProductId();

    SAVEPANEL = window.parent.GetSavePanel();
    COURSEPANEL = window.parent.GetCoursePanel();

    var baseInfo = Ext.create('Ext.form.Panel', {
        id: 'baseinfo',
        border: false,
        autoScroll: true,
        margin: '3 0 0 8',
        defaults: {
            labelWidth: 100,
            padding: '8 0 0 0'
        },
        items: [{//品牌
            xtype: 'combobox',
            allowBlank: false,
            hidden: true,
            id: 'brand_id',
            name: 'Brand_Id',
            colName: 'brand_id',
            store: brandStore,
            queryMode: 'local',
            displayField: 'brand_name',
            valueField: 'brand_id',
            typeAhead: true,
            forceSelection: true,
            emptyText: SELECT,
            fieldLabel: BRAND + '<span style="color:red">*</span>',
            listeners: {
                select: function () {
                    if (!PRODUCT_ID) {
                        var brand_id = Ext.getCmp("brand_id").getValue();
                        Ext.Ajax.request({
                            url: '/Product/GetProductsortNum',
                            params: {
                                brand_id: brand_id
                            },
                            success: function (response) {
                                var text = response.responseText;
                                Ext.getCmp("product_sort").setValue(text);
                            }
                        });
                    }
                }
            }
        }, {
            xtype: 'panel',
            columnWidth: 80,
            border: 0,
            layout: 'column',
            items: [{
                xtype: 'textfield',
                id: 'Prod_name',//商品名稱
                allowBlank: false,
                colName: 'Product_name',
                //disabled: window.parent.GetProductId() != "",//禁用 2014/09/16
                hidden: true,
                width: 400,
                name: 'Prod_Name',
                fieldLabel: PRODUCT_NAME + '<span style="color:red">*</span>'
            }, {
                xtype: 'textfield',
                margin: '0 0 0 10',
                id: 'prod_sz',//規格欄位
                allowBlank: true,
                //colName: 'Prod_sz',
                //hidden: true,
                width: 80,
                maxLength: 10,
                name: 'Prod_Sz'
            }, {// add by zhuoqin0830w  2015/06/26  添加商品失格欄位
                xtype: 'checkbox',
                margin: '0 0 0 10',
                boxLabel: OFF_GRADE,
                id: 'off-grade',
                width: 80,
                colName: 'off-grade',
                hidden: true
            }]
        }, {
            xtype: 'panel',
            columnWidth: .70,
            border: 0,
            layout: 'column',
            items: [{
                xtype: 'combobox',
                allowBlank: false,
                //hidden: true,
                id: 'product_type',//商品型態 add 2014/09/15
                name: 'Product_Type',
                colName: 'product_type',
                store: productTypeStore,
                queryMode: 'local',
                value: "0",//add 2014/09/18
                editable: false,
                displayField: 'parameterName',
                valueField: 'parameterCode',
                emptyText: SELECT,
                fieldLabel: PRODUCT_TYPE + '<span style="color:red">*</span>',
                listeners: {
                    change: function () {
                        if (Ext.getCmp('product_type').getValue() == 1) {
                            SAVEPANEL.insert(4, COURSEPANEL);
                            Ext.getCmp('course_name').allowBlank = false;
                            Ext.getCmp('course_name').show();
                        } else {
                            SAVEPANEL.remove(COURSEPANEL);
                            Ext.getCmp('course_name').allowBlank = true;
                            Ext.getCmp('course_name').hide();
                            window.parent.SetCourseId(0);
                        }
                    }
                }
            }, {
                xtype: 'combobox',
                allowBlank: true,
                hidden: true,
                id: 'course_name',//課程名稱 add 2013/03/10
                name: 'course_name',
                store: courseTypeStore,
                margin: '0 0 0 10 ',
                labelWidth: 70,
                //editable: false,
                queryMode: 'local',
                typeAhead: true,
                forceSelection: true,
                displayField: 'Course_Name',
                valueField: 'Course_Id',
                emptyText: SELECT,
                readOnly: PRODUCT_ID != '',
                fieldLabel: CURRICULUM_NAME + '<span style="color:red">*</span>',
                listeners: {
                    change: function () {
                        window.parent.SetCourseId(this.value);
                    }
                }
            }]
        }, {
            xtype: 'numberfield',
            decimalPrecision: 0,
            minValue: 0,
            value: 0,
            hidden: true,
            id: 'product_sort',//排序
            colName: 'product_sort',
            allowBlank: false,
            fieldLabel: PRODUCT_SORT,
            name: 'Product_Sort'
        }, {
            xtype: 'textfield',
            hidden: true,
            id: 'product_vendor_code',//廠商商品編號
            colName: 'product_vendor_code',
            fieldLabel: PRODUCT_VENDOR_CODE,
            name: 'Product_Vendor_Code'
        }, {
            xtype: 'panel',
            hidden: true,
            colName: 'product_start',//上架時間
            layout: 'hbox',
            id: 'product_start',
            allowBlank: false,
            border: false,
            items: [{
                xtype: 'datetimefield',
                disabledMin: true,
                disabledSec: true,
                format: 'Y-m-d H:i:s',
                id: 'p_start',
                fieldLabel: PRODUCT_START,
                allowBlank: false,
                //labelWidth: 80,
                value: tomRepeat,
                margin: '0 8 0 0',
                listeners: {
                    select: function (a, b, c) {
                        var Year = new Date(this.getValue()).getFullYear() + 10;
                        Ext.getCmp("pt_end").setValue(new Date(new Date(this.getValue()).setFullYear(Year)));
                    }
                }
            }]
        }, {
            xtype: 'panel',
            hidden: true,
            layout: 'hbox',
            border: false,
            id: 'product_end',
            colName: 'product_end',//下架時間
            items: [{
                xtype: 'datetimefield',
                disabledMin: true,
                disabledSec: true,
                format: 'Y-m-d H:i:s',
                id: 'pt_end',
                value: new Date(tomorrow.setFullYear(tomorrow.getFullYear() + 10)),
                allowBlank: false,
                fieldLabel: PRODUCT_END,
                //labelWidth: 80,
                margin: '0 8 0 0',
                listeners: {
                }
            }]
        }, {
            xtype: 'radiogroup',
            //hidden: true,
            colName: 'purchase_in_advance',
            id: 'purchase_in_advance',//是否預購 add by Jiajun 2015/07/30
            fieldLabel: PURCHASE_IN_ADVANCE,
            width: 200,
            defaults: {
                name: 'purchase_in_advance'
            },
            columns: 2,
            vertical: true,
            items: [
            { boxLabel: YES, inputValue: '1' },
            { boxLabel: NO, inputValue: '0', checked: true }],
            listeners: {
                change: function () {
                    if (Ext.getCmp('purchase_in_advance').getValue().purchase_in_advance == '1') {
                        Ext.getCmp('purchase_in_advance_start').setDisabled(false);
                        Ext.getCmp('blp').setDisabled(false);
                        Ext.getCmp('purchase_in_advance_end').setDisabled(false);
                        Ext.getCmp('e_time').setDisabled(false);
                    } else {
                        Ext.getCmp('purchase_in_advance_start').setDisabled(true);
                        Ext.getCmp('blp').setDisabled(true);
                        Ext.getCmp('purchase_in_advance_end').setDisabled(true);
                        Ext.getCmp('e_time').setDisabled(true);
                    }
                }
            }
        }, {
            xtype: 'container',
            layout: 'column',
            id: 'purchase_inAdvance',//預售起止時間
            items: [{
                xtype: 'datetimefield',
                disabledMin: true,
                disabledSec: true,
                format: 'Y-m-d H:i:s',
                id: 'purchase_in_advance_start',
                fieldLabel: PURCHASE_IN_ADVANCE_TIME,
                allowBlank: false,
                disabled: true,
                value: new Date(),
                minValue: new Date()
            }, {
                xtype: 'displayfield',
                value: '~ ',
                id: 'blp',
                disabled: true,
                margin: '0 5 0 5'
            }, {
                xtype: 'datetimefield',
                disabledMin: true,
                disabledSec: true,
                format: 'Y-m-d H:i:s',
                id: 'purchase_in_advance_end',
                allowBlank: false,
                disabled: true,
                value: tomRepeat,
                minValue: new Date()
            }, {
                xtype: 'datetimefield',
                disabledMin: true,
                disabledSec: true,
                disabled: true,
                format: 'Y-m-d H:i:s',
                id: 'e_time',
                fieldLabel: EXPECT_TIME,
                labelWidth: 80,
                margin: '0 8 0 15'
            }]
        }, {
            xtype: 'panel',
            hidden: true,
            layout: 'hbox',
            border: false,
            id: 'expect_time',//預計出貨時間
            colName: 'expect_time',
            items: [{
                xtype: 'textfield',
                hidden: true,
                width: 300,
                id: 'expect_msg',//預計出貨信息
                labelWidth: 110,
                fieldLabel: EXPECT_MSG,
                colName: 'expect_msg',
                name: 'expect_msg'
                //maxLength: 80,
                //maxLengthText:'此欄位最多輸入80個字'
            }]
        },
          {
              xtype: 'container',
              layout: 'column',
              hidden:true,
              id: 'jundge_combox',
              items: [{
                  xtype: 'radiogroup',
                  id: 'recommedde_jundge',//是否推薦商品屬性 add by dongya 2015/08/17
                  name: 'recommedde_jundge',
                  fieldLabel: RECOMMENDED_JUDGE_PRODUCT,
                  width: 200,
                  defaults: {
                      name: 'recommedde_jundge'
                  },
                  columns: 2,
                  vertical: true,
                  items: [
                  { boxLabel: YES, inputValue: '1' },
                  { boxLabel: NO, inputValue: '0', checked: true }],
                  listeners: {
                      change: function () {
                          if (Ext.getCmp('recommedde_jundge').getValue().recommedde_jundge == '1') {
                              Ext.getCmp('recommedde_time').show();//如果點擊了是  則顯示 
                              Ext.getCmp('recommedde_expend_day').show();
                              Ext.getCmp('checkall').show();
                              Ext.getCmp('noselect').show();
                          } else {
                              Ext.getCmp('recommedde_time').hide();  //如果點擊了否 則隱藏
                              Ext.getCmp('recommedde_expend_day').hide();
                              Ext.getCmp('recommedde_time').setValue(false);
                              Ext.getCmp('recommedde_expend_day').setValue(0);
                              Ext.getCmp('checkall').hide();
                              Ext.getCmp('noselect').hide();
                          }
                      }
                  }
              },
               {
                   xtype: 'numberfield',
                   id: 'recommedde_expend_day',
                   name: 'recommedde_expend_day',
                   fieldLabel: USED_TIME,
                   hidden: true,
                   allowBlank: false,
                   disabled: false,
                   allowDecimals: false,
                   width: 190,
                   maxValue: 1000000,
                   value: 0,
                   minValue: 0
                   ,
                   listeners: {
                       change: function () {
                           if (Ext.getCmp('recommedde_jundge').getValue().recommedde_jundge == '1') {
                               if (Ext.getCmp('recommedde_expend_day').getValue() > 1000000) {
                                   Ext.getCmp('recommedde_expend_day').setValue(1000000);
                               }
                           }
                       }
                   }
               }
               ,
                {
                    xtype: 'button',
                    text: '全選',
                    hidden:true,
                    id: 'checkall',
                    name:'checkall',
                    margin: '0 0 0 15',
                    handler: function () {
                        if (Ext.getCmp("recommedde_jundge").getValue().recommedde_jundge == 1) {
                            Ext.getCmp('recommedde_time_one').setValue(true);
                            Ext.getCmp('recommedde_time_two').setValue(true);
                            Ext.getCmp('recommedde_time_three').setValue(true);
                            Ext.getCmp('recommedde_time_four').setValue(true);
                            Ext.getCmp('recommedde_time_five').setValue(true);
                            Ext.getCmp('recommedde_time_six').setValue(true);
                            Ext.getCmp('recommedde_time_seven').setValue(true);
                            Ext.getCmp('recommedde_time_eight').setValue(true);
                            Ext.getCmp('recommedde_time_nine').setValue(true);
                            Ext.getCmp('recommedde_time_ten').setValue(true);
                            Ext.getCmp('recommedde_time_eleven').setValue(true);
                            Ext.getCmp('recommedde_time_twelve').setValue(true);
                        }
                    }
                },
        {
            xtype: 'button',
            text: '反選',
            hidden:true,
            id: 'noselect',
            name: 'noselect',
            margin: '0 0 0 15',
            handler: function () {
                if (Ext.getCmp("recommedde_jundge").getValue().recommedde_jundge == 1) {
                    if (Ext.getCmp("recommedde_time_one").getValue() == true) {
                        Ext.getCmp("recommedde_time_one").setValue(false)
                    }
                    else {
                        Ext.getCmp("recommedde_time_one").setValue(true)
                    }
                    if (Ext.getCmp("recommedde_time_two").getValue() == true) {
                        Ext.getCmp("recommedde_time_two").setValue(false)
                    }
                    else {
                        Ext.getCmp("recommedde_time_two").setValue(true)
                    }
                    if (Ext.getCmp("recommedde_time_three").getValue() == true) {
                        Ext.getCmp("recommedde_time_three").setValue(false)
                    }
                    else {
                        Ext.getCmp("recommedde_time_three").setValue(true)
                    }
                    if (Ext.getCmp("recommedde_time_four").getValue() == true) {
                        Ext.getCmp("recommedde_time_four").setValue(false)
                    }
                    else {
                        Ext.getCmp("recommedde_time_four").setValue(true)
                    }
                    if (Ext.getCmp("recommedde_time_five").getValue() == true) {
                        Ext.getCmp("recommedde_time_five").setValue(false)
                    }
                    else {
                        Ext.getCmp("recommedde_time_five").setValue(true)
                    }
                    if (Ext.getCmp("recommedde_time_six").getValue() == true) {
                        Ext.getCmp("recommedde_time_six").setValue(false)
                    }
                    else {
                        Ext.getCmp("recommedde_time_six").setValue(true)
                    }
                    if (Ext.getCmp("recommedde_time_seven").getValue() == true) {
                        Ext.getCmp("recommedde_time_seven").setValue(false)
                    }
                    else {
                        Ext.getCmp("recommedde_time_seven").setValue(true)
                    }
                    if (Ext.getCmp("recommedde_time_eight").getValue() == true) {
                        Ext.getCmp("recommedde_time_eight").setValue(false)
                    }
                    else {
                        Ext.getCmp("recommedde_time_eight").setValue(true)
                    }
                    if (Ext.getCmp("recommedde_time_nine").getValue() == true) {
                        Ext.getCmp("recommedde_time_nine").setValue(false)
                    }
                    else {
                        Ext.getCmp("recommedde_time_nine").setValue(true)
                    }
                    if (Ext.getCmp("recommedde_time_ten").getValue() == true) {
                        Ext.getCmp("recommedde_time_ten").setValue(false)
                    }
                    else {
                        Ext.getCmp("recommedde_time_ten").setValue(true)
                    }
                    if (Ext.getCmp("recommedde_time_eleven").getValue() == true) {
                        Ext.getCmp("recommedde_time_eleven").setValue(false)
                    }
                    else {
                        Ext.getCmp("recommedde_time_eleven").setValue(true)
                    }
                    if (Ext.getCmp("recommedde_time_twelve").getValue() == true) {
                        Ext.getCmp("recommedde_time_twelve").setValue(false)
                    }
                    else {
                        Ext.getCmp("recommedde_time_twelve").setValue(true)
                    }
                }
            }
        }
              ]
          },
           {
               xtype: 'checkboxgroup',
               id: 'recommedde_time',//是否推薦商品屬性 add by dongya 2015/08/17
               name: 'recommedde_time',
               fieldLabel: "推薦月份設定",
               hidden: true,
               width: 800,
               columns: 6,
               vertical: true,
               allowBlank: true,
               items: [
                { boxLabel: '一月', id: 'recommedde_time_one', inputValue: '1' },
                  { boxLabel: '二月', id: 'recommedde_time_two', inputValue: '2' },
                  { boxLabel: '三月', id: 'recommedde_time_three', inputValue: '3' },
                  { boxLabel: '四月', id: 'recommedde_time_four', inputValue: '4' },
                  { boxLabel: '五月', id: 'recommedde_time_five', inputValue: '5' },
                  { boxLabel: '六月', id: 'recommedde_time_six', inputValue: '6' },
                  { boxLabel: '七月', id: 'recommedde_time_seven', inputValue: '7' },
                  { boxLabel: '八月', id: 'recommedde_time_eight', inputValue: '8' },
                  { boxLabel: '九月', id: 'recommedde_time_nine', inputValue: '9' },
                  { boxLabel: '十月', id: 'recommedde_time_ten', inputValue: '10' },
                  { boxLabel: '十一月', id: 'recommedde_time_eleven', inputValue: '11' },
                  { boxLabel: '十二月', id: 'recommedde_time_twelve', inputValue: '12' }
               ]
               //,
               //listeners: {
               //    change: function () {
               //        if (Ext.getCmp('recommedde_jundge').getValue().recommedde_jundge == '1') {
               //            if (Ext.getCmp('recommedde_time').getChecked() == 0) {
               //                if (Ext.getCmp('recommedde_expend_day').getValue() > 0) {
               //                    Ext.Msg.alert(PROMPT, MONTH_SET_NOEMPTY);//推薦週期不允許為空
               //                    Ext.getCmp('recommedde_expend_day').setValue(0);
               //                }
               //            }
               //        }
               //    }
               //}
           },

        {
            xtype: 'panel',
            layout: 'hbox',
            border: false,
            items: [{
                xtype: 'combobox',
                id: 'schedule_id',
                name: 'schedule_id',
                store: tierStore,//選擇排程
                //margin: '0 0 0 8 ',
                //labelWidth: 60,
                queryMode: 'local',
                editable: false,
                hidden: PRODUCT_ID == '' ? true : false,
                displayField: 'schedule_name',
                valueField: 'schedule_Id',
                emptyText: SELECT,
                fieldLabel: SCHEDULE_SELECT,
                listConfig: {
                    getInnerTpl: function () {
                        return '<div>{schedule_name}</div>';
                    }
                },
                listeners: {
                    select: function (record) {
                        Ext.getCmp("desc").setText(record.valueModels[0].data.desc)
                    }
                }
            }, {
                xtype: 'label',
                id: 'desc',
                margin: '0 0 0 15'
            }]
        }, {
            xtype: 'radiogroup',
            hidden: true,
            id: 'product_freight_set',//運送方式
            colName: 'product_freight_set',
            fieldLabel: PRODUCT_FREIGHT_SET,
            width: 500,
            name: 'Product_Freight_Set'
        }, {//edit by zhuoqin0830w  2015/03/16
            xtype: 'panel',
            //hidden: true,
            layout: 'hbox',
            border: false,
            padding: '0 0 4 0',
            colName: 'product_extra_days',
            id: 'product_extra_days',
            items: [{
                xtype: 'radiogroup',
                hidden: true,
                id: 'product_mode',
                colName: 'product_mode',
                name: 'Product_Mode',
                fieldLabel: PRODUCT_MODE,
                margin: '3 11 0 0',
                width: 250
            }, {//寄倉天數/調度天數 
                xtype: 'numberfield',
                decimalPrecision: 0,
                minValue: 0,
                hidden: true,
                id: 'extra_days',
                margin: '1 0 0 0',
                //colName: 'extra_days',
                allowBlank: true,
                fieldLabel: CONTROL_EXTRA_DAYS,
                name: 'Extra_days',
                width: 200
            }]
        }, {//add by zhuoqin0830w  2015/03/16
            xtype: 'panel',
            //hidden: true,
            layout: 'hbox',
            border: false,
            items: [{//供應商出貨天數
                xtype: 'numberfield',
                decimalPrecision: 0,
                minValue: 0,
                value: 0,
                margin: '0 35 0 0',
                hidden: true,
                id: 'deliver_days',
                //colName: 'deliver_days',
                allowBlank: false,
                fieldLabel: DELIVER_DAYS,
                name: 'Deliver_days',
                width: 200
            }, {//最小採購數量
                xtype: 'numberfield',
                decimalPrecision: 0,
                minValue: 0,
                value: 1,
                hidden: true,
                margin: '0 8 0 0',
                id: 'min_purchase_amount',
                colName: 'min_purchase_amount',
                allowBlank: false,
                fieldLabel: MIN_PURCHASE_AMOUNT,
                name: 'Min_purchase_amount',
                width: 200
            }]
        }, {//add by zhuoqin0830w  2015/03/16
            //採購調整系數
            xtype: 'numberfield',
            decimalPrecision: 0,
            minValue: 0,
            margin: '0 35 0 0',
            hidden: true,
            id: 'safe_stock_amount',
            value: 1,
            colName: 'safe_stock_amount',
            allowBlank: false,
            fieldLabel: SAFE_STOCK_AMOUNT,
            name: 'Safe_stock_amount',
            width: 200

        }, {
            xtype: 'radiogroup',
            hidden: true,
            id: 'tax_type',//營業稅
            fieldLabel: TAX_TYPE,
            colName: 'tax_type',
            width: 200,
            defaults: {
                name: 'Tax_Type'
            },
            columns: 2,
            vertical: true,
            items: [
            { boxLabel: TAX_TYPE1, inputValue: '1', checked: true },
            { boxLabel: TAX_TYPE2, inputValue: '3' }]
        }, {
            xtype: 'radiogroup',
            hidden: true,
            id: 'show_in_deliver',//顯示於出貨單中 add by Jiajun 2014/09/15
            fieldLabel: SHOE_IN_DELIVER,
            colName: 'show_in_deliver',
            width: 200,
            defaults: {
                name: 'Show_In_Deliver'
            },
            columns: 2,
            vertical: true,
            items: [
            { boxLabel: YES, inputValue: '1', checked: true },
            { boxLabel: NO, inputValue: '0' }
            ]
        }, {
            xtype: 'radiogroup',
            hidden: true,
            id: 'prepaid',//已買斷的商品 add 2014/09/15
            fieldLabel: PREPAID,
            colName: 'prepaid',
            width: 200,
            defaults: {
                name: 'Prepaid'
            },
            columns: 2,
            vertical: true,
            items: [{
                boxLabel: YES, inputValue: '1', listeners: { //edit by Jiajun 2014.09.29
                    change: function (radio, newValue, oldValue) {
                        if (isChange) {
                            Ext.Msg.alert(INFORMATION, BAG_CHECK_PREPAID);
                        }
                        isChange = true;
                    }
                }
            },
            { boxLabel: NO, inputValue: '0', checked: true }//edit 2014/08/17
            ]
        }, {
            xtype: 'radiogroup',
            hidden: true,
            id: 'process_type',//配送系統 add 2014/09/15
            fieldLabel: PROCESS_TYPE,
            colName: 'process_type',
            width: 400,
            defaults: {
                name: 'Process_Type'
            },
            columns: 4,
            vertical: true,
            items: [
            { boxLabel: PRO_TYPE_WL, inputValue: '0', checked: true },//0:物流配送, 1:電子郵件, 2:簡訊, 99:系統
            { boxLabel: PRO_TYPE_DZ, inputValue: '1' },
            { boxLabel: PRO_TYPE_JX, inputValue: '2' },
            { boxLabel: PRO_TYPE_XT, inputValue: '99' }
            ]
        }, {
            xtype: 'button',
            iconCls: 'icon_rewind',
            text: SAVE,
            hidden: true,
            handler: function () { }
        }, {
            xtype: 'displayfield',
            id: 'combination',
            name: 'combination',
            hidden: window.parent.GetProductId() == "",
            value: SILGLE_PROD,
            fieldLabel: COMBINATION
        }],
        listeners: {
            beforerender: function () {
                //檢測臨時表中是否有數據
                Ext.Ajax.request({
                    url: '/Product/QueryProduct',
                    method: 'post',
                    params: {
                        "product_id": window.parent.GetProductId(),
                        OldProductId: OLD_PRODUCT_ID
                    },
                    success: function (response) {
                        //判斷是單一商品新增還是修改  add by zhuoqin0830w  2015/06/30
                        if (window.parent.GetProductId() == "") {
                            Ext.getCmp("off-grade").hide();
                        }
                        //商品複製
                        var reStr = eval("(" + response.responseText + ")");
                        if (reStr.data != null && window.parent.GetProductId() == "" && OLD_PRODUCT_ID == '') {
                            window.parent.Is_Continue();
                            //去掉右上角關閉按鈕
                        }
                        else {
                            Page_Load();
                        }
                    }
                });
            }
        }
    });

    Ext.create('Ext.Viewport', {
        layout: 'fit',
        items: [baseInfo],
        border: false,
        style: {
            padding: '5 0 0 5'
        },
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                this.doLayout();
            },
            afterrender: function () {
                window.parent.updateAuth(baseInfo, 'colName');
            }
        }
    });


    //運送方式
    var product_freight = Ext.get("freight").dom.value;
    Ext.getCmp("product_freight_set").update(product_freight);

    //var extra_days = Ext.getCmp("extra_days");
    //出貨方式
    var product_mode = Ext.get("mode").dom.value;
    Ext.getCmp("product_mode").update(product_mode);

    $(":radio[name='product_mode']").bind('click', function () {
        if (this.checked && this.value != '2') {
            Ext.Msg.alert(INFORMATION, BAG_CHECK_INFORMATION);
        }
    });
});

//時間   edit by jiajun 2014/12/31
function Tomorrow() {
    var d;
    d = new Date(); // 创建 Date 对象。 
    d.setDate(d.getDate() + 1);
    return d;
}

//頁面加載
function Page_Load() {
    /**/
    $("#off-grade input[type=button]").click(function () {
        var off_grade = Ext.getCmp('off-grade');
        var oldValue = off_grade.getValue();
        Ext.Msg.confirm(ALERT_MESSAGE, AFFIRM_FIX,
        function (button) {
            if (button == 'no') {
                off_grade.setValue(!oldValue);
            }
        });
    });

    $(".x-tool-close").show();
    Ext.getCmp("baseinfo").getForm().load({
        type: 'ajax',
        url: '/Product/QueryProduct',
        actionMethods: 'post',
        params: {
            "ProductId": Ext.htmlEncode(window.parent.GetProductId()),
            OldProductId: OLD_PRODUCT_ID
        },
        success: function (response, opts) {
            var resText = eval("(" + opts.response.responseText + ")");
            productInfo = resText;//給productInfo賦值,用於其他代碼的條件
            tierStore.load({
                params: { 'vendorId': productInfo.data.Vendor_Id, 'product_id': productInfo.data.Product_Id },
                callback: function (record, operation, success) {
                    //Ext.getCmp("schedule_id").setValue(tierStore.getAt(0).data.schedule_Id);
                    tierStore.insert(0, { schedule_Id: '0', schedule_name: SCHEDULE_CANCEL });
                }
            });
            if (OLD_PRODUCT_ID)
                Ext.getCmp('brand_id').fireEvent('select');

            if (resText.data.Product_Start != "0") {
                var date_start = new Date(resText.data.Product_Start * 1000);
                Ext.getCmp("p_start").setValue(date_start);
            }

            if (resText.data.Product_End != "0") {
                var date_end = new Date(resText.data.Product_End * 1000);
                Ext.getCmp("pt_end").setValue(date_end);
            }

            if (resText.data.Expect_Time != "0") {
                var expect_time = new Date(resText.data.Expect_Time * 1000);
                Ext.getCmp("e_time").setValue(expect_time);
            }


            //add by Jiajun 2014.09.30
            if (resText.data.Product_Status != "0") {
                Ext.getCmp("process_type").disable(true);
                Ext.getCmp("product_type").disable(true);
            }

            //add by zhuoqin0830w  2015/06/25 下架和下架不販售才會顯示失格
            if (resText.data.Product_Status != 6 && resText.data.Product_Status != 99) {
                Ext.getCmp("off-grade").setDisabled(true);
            }
            Ext.getCmp("off-grade").setValue(resText.data.off_grade);


            $("#product_freight" + resText.data.Product_Freight_Set + "").attr("checked", "checked");

            $("#product_mode" + resText.data.Product_Mode + "").attr("checked", "checked");

            $("#tax" + resText.data.Tax_Type + "").attr("checked", "checked");

            Ext.getCmp("tax_type").setValue(resText.data.Tax_Type);
            //add 2014/09/16
            Ext.getCmp("show_in_deliver").setValue(resText.data.Show_In_Deliver);//顯示於出貨單中

            if (resText.data.Prepaid == 0) {
                isChange = true;
            }
            Ext.getCmp("prepaid").setValue(resText.data.Prepaid);//已買斷商品

            Ext.getCmp("process_type").setValue(resText.data.Process_Type);//配送系統
            Ext.getCmp("product_type").setValue(resText.data.Product_Type);//商品形態

            if (PRODUCT_ID != '') { //add 2014/04
                if (Ext.getCmp('product_type').getValue() != 1) {
                    productTypeStore.removeAt(productTypeStore.find('parameterCode', 1));
                }
            }

            Ext.getCmp("schedule_id").setValue(resText.data.Schedule_Id);//排程ID
            if (Ext.getCmp("schedule_id") && Ext.getCmp("schedule_id").getValue() != 0 || Ext.getCmp("schedule_id").getValue() != null) { //add 2014/04/22
                Ext.getCmp("desc").setText(resText.data.Desc)
            }
            //Ext.getCmp("Prod_name").setValue(resText.data.Prod_Name_Simple);
            //add 2014/11/18
            Ext.getCmp("prod_sz").setValue(resText.data.Prod_Sz);

            window.parent.SetCourseId(resText.data.Course_Id);
            Ext.getCmp("course_name").setValue(resText.data.Course_Id);//課程

            //add by zhuoqin0830w  2015/03/16  商品基本資料新增欄位
            Ext.getCmp("deliver_days").setValue(resText.data.Deliver_Days);//供應商出貨天數
            Ext.getCmp("min_purchase_amount").setValue(resText.data.Min_Purchase_Amount);//最小採購數量
            Ext.getCmp("safe_stock_amount").setValue(resText.data.Safe_Stock_Amount);//安全存量細數 
            //Ext.getCmp("extra_days").setValue(resText.data.Extra_Days);//寄倉天數/調度天數

            //add by Jiajun 2015/07/30
            Ext.getCmp("purchase_in_advance").setValue({ "purchase_in_advance": resText.data.purchase_in_advance });

            if (resText.data.purchase_in_advance_start != '0') {
                Ext.getCmp("purchase_in_advance_start").setValue(new Date(resText.data.purchase_in_advance_start * 1000));
            } else {
                Ext.getCmp("purchase_in_advance_start").setValue(new Date());
            }

            if (resText.data.purchase_in_advance_end != '0') {
                Ext.getCmp("purchase_in_advance_end").setValue(new Date(resText.data.purchase_in_advance_end * 1000));
            } else {
                Ext.getCmp("purchase_in_advance_end").setValue(Tomorrow());
            }

            //add by dongya 2015/08/17
            if (resText.data.expend_day == 0 && resText.data.months == "") {
                Ext.getCmp("recommedde_jundge").setValue({ "recommedde_jundge": 0 });
                Ext.getCmp('recommedde_time').hide();//如果點擊的是否,則不顯示
                Ext.getCmp('recommedde_expend_day').hide();
            }
            else {
                Ext.getCmp('recommedde_time').show();//如果點擊的是否,則不顯示
                Ext.getCmp('recommedde_expend_day').show();
                Ext.getCmp("recommedde_jundge").setValue({ "recommedde_jundge": 1 });
                var arraylist = resText.data.months.split(',');
                for (var a = 0; a < arraylist.length; a++) {
                    if (arraylist[a] == 1) {
                        Ext.getCmp("recommedde_time_one").setValue(true);
                    }
                    if (arraylist[a] == 2) {
                        Ext.getCmp("recommedde_time_two").setValue(true);
                    }
                    if (arraylist[a] == 3) {
                        Ext.getCmp("recommedde_time_three").setValue(true);
                    }
                    if (arraylist[a] == 4) {
                        Ext.getCmp("recommedde_time_four").setValue(true);
                    }
                    if (arraylist[a] == 5) {
                        Ext.getCmp("recommedde_time_five").setValue(true);
                    }
                    if (arraylist[a] == 6) {
                        Ext.getCmp("recommedde_time_six").setValue(true);
                    }
                    if (arraylist[a] == 7) {
                        Ext.getCmp("recommedde_time_seven").setValue(true);
                    }
                    if (arraylist[a] == 8) {
                        Ext.getCmp("recommedde_time_eight").setValue(true);
                    }
                    if (arraylist[a] == 9) {
                        Ext.getCmp("recommedde_time_nine").setValue(true);
                    }
                    if (arraylist[a] == 10) {
                        Ext.getCmp("recommedde_time_ten").setValue(true);
                    }
                    if (arraylist[a] == 11) {
                        Ext.getCmp("recommedde_time_eleven").setValue(true);
                    }
                    if (arraylist[a] == 12) {
                        Ext.getCmp("recommedde_time_twelve").setValue(true);
                    }
                }
                Ext.getCmp("recommedde_expend_day").setValue(resText.data.expend_day);
            }

        }
    })
    /**/
}

//驗證表單
function validateForm(product_start, product_end, expect_time, purchase_in_advance_start, purchase_in_advance_end) {
    if (!Ext.getCmp('baseinfo').getForm().isValid()) {
        return;
    }
    if (!Ext.getCmp("brand_id").isValid() || Ext.getCmp("brand_id").getValue() == SELECT) {
        Ext.Msg.alert(PROMPT, BRAND_NOT_NULL);
        return;
    }
    if (!Ext.getCmp("Prod_name").isValid()) {
        Ext.Msg.alert(PROMPT, PRODUCT_NAME_NOT_NULL);
        return;
    }
    if (!Ext.getCmp("product_sort").isValid()) {
        Ext.Msg.alert(PROMPT, PRDUCT_SORT);
        return;
    }
    if (!Ext.getCmp("product_vendor_code").isValid()) {
        Ext.Msg.alert(PROMPT, PRODUCT_VENDOR_CODE_NOT_NULL);
        return;
    }
    //add 2014/09/15
    if (!Ext.getCmp("product_type").isValid()) {
        Ext.Msg.alert(PROMPT, PRODUCT_TYPE_NOT_NULL);
        return;
    }
    if (window.parent.GetProductId() == "") {
        if (Ext.getCmp("p_start").rawValue != "" && product_start < new Date()) {
            Ext.Msg.alert(PROMPT, PRODUCT_START_LIMIT);
            return;
        }
        if (Ext.getCmp("pt_end").rawValue != "" && product_end < new Date()) {
            Ext.Msg.alert(PROMPT, PRODUCT_END_LIMIT);
            return;
        }
        if (Ext.getCmp("e_time").rawValue != "" && expect_time < new Date()) {
            Ext.Msg.alert(PROMPT, EXPECT_TIME_LIMIT);
            return;
        }
    }

    if (product_end != "0" && product_end < product_start) {
        Ext.Msg.alert(PROMPT, START_END_LIMIT);
        return;
    }

    //add by Jiajun 2015/07/30
    if (Ext.getCmp('purchase_in_advance').getValue().purchase_in_advance == '1') {
        if (purchase_in_advance_start >= purchase_in_advance_end) {
            Ext.Msg.alert(PROMPT, START_END_LIMIT_INADVANCE);
            return;
        }
    }

    //若填寫預計出貨時間則預計出貨信息必填
    if (Ext.getCmp("e_time").rawValue != "") {
        if (Ext.getCmp("expect_msg").getValue() == "") { Ext.Msg.alert(PROMPT, PLEASE_WRITE_SHIPMENT_MESSAGE); return; }
    }

    //add by dongya 2015/08/25
    if (Ext.getCmp("recommedde_jundge").getValue().recommedde_jundge == 1) {
        if (Ext.getCmp('recommedde_time').getChecked() == 0 || Ext.getCmp("recommedde_expend_day").getValue() == 0) {
            Ext.Msg.alert(PROMPT, MONTH_SET_USED_TIME_NOEMPTY);
            return;
        }
    }
    return true;
}

//保存表單
function save(functionid) {
    var product_start = "0";
    var product_end = "0";
    var expect_time = "0";

    if (Ext.getCmp("p_start").rawValue != "") {
        product_start = Ext.getCmp("p_start").rawValue;
    }
    if (Ext.getCmp("pt_end").rawValue != "") {
        product_end = Ext.getCmp("pt_end").rawValue;
    }
    if (Ext.getCmp("e_time").rawValue != "") {
        expect_time = Ext.getCmp("e_time").rawValue;
    }

    //add by Jiajun 2014/07/30
    purchase_in_advance_start = Ext.getCmp("purchase_in_advance_start").getValue() / 1000;
    purchase_in_advance_end = Ext.getCmp("purchase_in_advance_end").getValue() / 1000;

    var retVal = true;
    if (!validateForm(product_start, product_end, expect_time, purchase_in_advance_start, purchase_in_advance_end)) {
        window.parent.setMoveEnable(true);
        return false;
    }
    if (!functionid) {
        functionid = '';
    }
    //add by dongya 2015/08/26
    var recommededcheckall = "";
    if (Ext.getCmp("recommedde_jundge").getValue().recommedde_jundge == 1) {
        if (Ext.getCmp("recommedde_time_one").getValue() == true) {
            recommededcheckall = recommededcheckall + "1,";
        }
        if (Ext.getCmp("recommedde_time_two").getValue() == true) {
            recommededcheckall = recommededcheckall + "2,";
        }
        if (Ext.getCmp("recommedde_time_three").getValue() == true) {
            recommededcheckall = recommededcheckall + "3,";
        }
        if (Ext.getCmp("recommedde_time_four").getValue() == true) {
            recommededcheckall = recommededcheckall + "4,";
        }
        if (Ext.getCmp("recommedde_time_five").getValue() == true) {
            recommededcheckall = recommededcheckall + "5,";
        }
        if (Ext.getCmp("recommedde_time_six").getValue() == true) {
            recommededcheckall = recommededcheckall + "6,";
        }
        if (Ext.getCmp("recommedde_time_seven").getValue() == true) {
            recommededcheckall = recommededcheckall + "7,";
        }
        if (Ext.getCmp("recommedde_time_eight").getValue() == true) {
            recommededcheckall = recommededcheckall + "8,";
        }
        if (Ext.getCmp("recommedde_time_nine").getValue() == true) {
            recommededcheckall = recommededcheckall + "9,";
        }
        if (Ext.getCmp("recommedde_time_ten").getValue() == true) {
            recommededcheckall = recommededcheckall + "10,";
        }
        if (Ext.getCmp("recommedde_time_eleven").getValue() == true) {
            recommededcheckall = recommededcheckall + "11,";
        }
        if (Ext.getCmp("recommedde_time_twelve").getValue() == true) {
            recommededcheckall = recommededcheckall + "12,";
        }
    }
    //儲存數據
    Ext.Ajax.request({
        url: '/Product/SaveBaseInfo',
        method: 'POST',
        async: false,
        params: {
            OldProductId: OLD_PRODUCT_ID,
            "product_id": Ext.htmlEncode(window.parent.GetProductId()),
            "brand_id": Ext.htmlEncode(Ext.getCmp("brand_id").getValue()),
            "prod_name": Ext.htmlEncode(Ext.getCmp("Prod_name").getValue()),
            "product_sort": Ext.htmlEncode(Ext.getCmp("product_sort").getValue()),
            "product_vendor_code": Ext.htmlEncode(Ext.getCmp("product_vendor_code").getValue()),
            "product_start": product_start,
            "product_end": product_end,
            "expect_time": expect_time,
            "product_freight_set": Ext.htmlEncode($(":radio[name='product_freight']:checked")[0].value),
            "product_mode": Ext.htmlEncode($(":radio[name='product_mode']:checked")[0].value),
            "tax_type": Ext.htmlEncode(Ext.getCmp("tax_type").getValue().Tax_Type),
            "expect_msg": Ext.htmlEncode(Ext.getCmp('expect_msg').getValue()),

            //add 2014/09/15
            "show_in_deliver": Ext.htmlEncode(Ext.getCmp("show_in_deliver").getValue().Show_In_Deliver),
            "prepaid": Ext.htmlEncode(Ext.getCmp("prepaid").getValue().Prepaid),
            "process_type": Ext.htmlEncode(Ext.getCmp("process_type").getValue().Process_Type),
            "product_type": Ext.htmlEncode(Ext.getCmp("product_type").getValue()),

            //add 2014/11/18
            "Prod_Sz": Ext.htmlEncode(Ext.getCmp("prod_sz").getValue()),

            //add 2015/03/10
            "course_id": Ext.htmlEncode(Ext.getCmp("course_name").getValue()),
            //add 2015/03/26
            "schedule_id": Ext.htmlEncode(Ext.getCmp("schedule_id").getValue()),

            "function": functionid,
            "batch": window.parent.GetBatchNo(),

            //add by zhuoqin0830w  增加新的欄位  2015/03/17
            "deliver_days": Ext.htmlEncode(Ext.getCmp("deliver_days").getValue()),
            "min_purchase_amount": Ext.htmlEncode(Ext.getCmp("min_purchase_amount").getValue()),
            "safe_stock_amount": Ext.htmlEncode(Ext.getCmp("safe_stock_amount").getValue()),
            //add by zhuoqin0830w 2015/06/25
            "off-grade": Ext.getCmp("off-grade").getValue() ? 1 : 0,

            //add by Jiajun 2014/07/30
            "purchase_in_advance": Ext.htmlEncode(Ext.getCmp("purchase_in_advance").getValue().purchase_in_advance),
            "purchase_in_advance_start": purchase_in_advance_start,
            "purchase_in_advance_end": purchase_in_advance_end,
            //add by dongya 2015/08/17
            "recommedde_jundge": Ext.htmlEncode(Ext.getCmp("recommedde_jundge").getValue().recommedde_jundge),
            "recommededcheckall": recommededcheckall,
            "recommedde_expend_day": Ext.htmlEncode(Ext.getCmp("recommedde_expend_day").getValue())
        }, success: function (msg) {
            var resMsg = eval("(" + msg.responseText + ")");
            if (resMsg.success == true && resMsg.msg != null) {
                Ext.Msg.alert(PROMPT, resMsg.msg);
            }
            if (resMsg.success == false) {
                Ext.Msg.alert(PROMPT, resMsg.msg);
                retVal = false;
                window.parent.setMoveEnable(true);
            }
        }
    });
    return retVal;
}