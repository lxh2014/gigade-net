var a;
var columnCount_freight;
var columnCount_mode;
var PRODUCT_ID, OLD_PRODUCT_ID = '';
var startDate;
var isEdit;

//商品組合類型Model
Ext.define("gigade.ComboType", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
})

//商品組合類型store
var ComboStore = Ext.create("Ext.data.Store", {
    model: 'gigade.ComboType',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=combo_type',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
})
//品牌Model
Ext.define("gigade.Brand", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "brand_id", type: "string" },
        { name: "brand_name", type: "string"}]
});



//品牌store
var brandStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Brand',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/VendorBrand/QueryBrand",
        actionMethods: 'post',
        noCache: false,
        getMethod: function () { return 'get'; },
        reader: {
            type: 'json',
            root: 'item'
        }
    }
});

Ext.onReady(function () {
    PRODUCT_ID = window.parent.GetProductId();
    OLD_PRODUCT_ID = window.parent.GetCopyProductId();
    isEdit = window.parent.GetIsEdit();

    var tomorrow = Tomorrow();
    var tomRepeat = Tomorrow();

    ComboStore.load({
        callback: function () {
            ComboStore.removeAt(ComboStore.find("parameterCode", 1));
        }
    });

    var baseInfo = Ext.create('Ext.form.Panel', {
        id: 'baseInfo',
        border: false,
        autoScroll: true,
        defaults: {
            labelWidth: 80,
            padding: '8 0 0 0'
        },
        items: [{
            xtype: 'combobox',
            allowBlank: false,
            //  hidden: true,
            id: 'brand_id',
            name: 'Brand_Id',
            store: brandStore,
            colName: 'brand_id',
            queryMode: 'local',
            displayField: 'brand_name',
            valueField: 'brand_id',
            typeAhead: true,
            forceSelection: true,
            emptyText: SELECT,
            fieldLabel: BRAND + '<span style="color:red">*</span>'
        }, {
            xtype: 'textfield',
            id: 'Product_name',
            colName: 'Product_name',
            allowBlank: false,
            //  hidden: true,
            width: 400,
            name: 'Product_Name',
            fieldLabel: PRODUCT_NAME + '<span style="color:red">*</span>'
        }, {
            xtype: 'numberfield',
            minValue: 0,
            value: 0,
            //  hidden: true,
            id: 'product_sort',
            decimalPrecision: 0,
            allowBlank: false,
            fieldLabel: PRODUCT_SORT,
            colName: 'product_sort',
            maxValue: 9999,
            name: 'Product_Sort'
        }, {
            xtype: 'textfield',
            //  hidden: true,
            id: 'product_vendor_code',
            fieldLabel: PRODUCT_VENDOR_CODE,
            colName: 'product_vendor_code',
            name: 'Product_Vendor_Code'
        }, {
            xtype: 'panel',
            //  hidden: true,
            layout: 'hbox',
            id: 'product_start',
            colName: 'product_start',
            allowBlank: false,
            border: false,
            items: [
        {
            xtype: 'datetimefield',
            disabledMin: true,
            disabledSec: true,
            format: 'Y-m-d H:i:s',
            id: 'p_start',
            value: tomRepeat,
            allowBlank: false,
            fieldLabel: PRODUCT_START,
            labelWidth: 80,
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
            //  hidden: true,
            layout: 'hbox',
            border: false,
            colName: 'product_end',
            id: 'product_end',
            items: [
        {
            xtype: 'datetimefield',
            disabledMin: true,
            disabledSec: true,
            format: 'Y-m-d H:i:s',
            id: 'pt_end',
            fieldLabel: PRODUCT_END,
            value: new Date(tomorrow.setFullYear(tomorrow.getFullYear() + 10)),
            allowBlank: false,
            labelWidth: 80,
            margin: '0 8 0 0',
            listeners: {

        }
    }
    ]
        }, {
            xtype: 'panel',
            //  hidden: true,
            layout: 'hbox',
            border: false,
            colName: 'expect_time',
            id: 'expect_time',
            items: [
        {
            xtype: 'datetimefield',
            disabledMin: true,
            disabledSec: true,
            id: 'e_time',
            format: 'Y-m-d H:i:s',
            fieldLabel: EXPECT_TIME,
            labelWidth: 80,
            margin: '0 8 0 0'
        }, {
            xtype: 'textfield',
            //  hidden: true,
            width: 300,
            id: 'expect_msg',
            labelWidth: 80,
            fieldLabel: EXPECT_MSG,
            colName: 'expect_msg',
            name: 'expect_msg'
        }
    ]
        }, {
            xtype: 'radiogroup',
            //  hidden: true,
            id: 'product_freight_set',
            fieldLabel: PRODUCT_FREIGHT_SET,
            colName: 'product_freight_set',
            width: 500,
            name: 'Product_Freight_Set'
        }, {
            xtype: 'radiogroup',
            //  hidden: true,
            id: 'product_mode',
            colName: 'product_mode',
            name: 'Product_Mode',
            fieldLabel: PRODUCT_MODE,
            width: 250
        }, {
            xtype: 'radiogroup',
            //  hidden: true,
            id: 'tax_type',
            fieldLabel: TAX_TYPE,
            width: 200,
            colName: 'tax_type',
            defaults: {
                name: 'Tax_Type'
            },
            columns: 2,
            vertical: true,
            items: [
            { boxLabel: TAX_TYPE1, inputValue: '1', checked: true },
            { boxLabel: TAX_TYPE2, inputValue: '3' }
        ]

        },
        {
            xtype: 'panel',
            border: false,
            //  hidden: true,
            id: 'comboType',
            colName: 'comboType',
            items:
         [{
             xtype: 'combobox',
             allowBlank: false,
             hidden: isEdit == "true",
             id: 'combination',
             name: 'Combination',
             store: ComboStore,
             displayField: 'parameterName',
             valueField: 'parameterCode',
             editable: false,
             queryMode: 'local',
             value: SELECT,
             fieldLabel: COMBINATION,
             listeners: {
                 select: function () {
                     Ext.Ajax.request({
                         url: '/VendorProductCombo/combSpecTempDelete',
                         method: 'post',
                         params: {
                             ProductId: window.parent.GetProductId(),
                             OldProductId: window.parent.GetCopyProductId()
                         }
                     });
                 }


             }
         }, {
             xtype: 'displayfield',
             width: 500,
             hidden: isEdit == 'true',
             html: '<span style="color:red;font-weight:bold;font-size:11px;font-family:雅黑;float:left">' + ATTENTION + '</span>'
         }, {
             xtype: 'displayfield',
             width: 400,
             hidden: isEdit == 'false',
             fieldLabel: COMBINATION,
             id: 'disCombination'
         }]
        }],
        listeners: {
            beforerender: function () {
                //檢測臨時表中是否有數據
                Ext.Ajax.request({
                    url: '/VendorProductCombo/QueryProduct',
                    method: 'post',
                    params: {
                        "product_id": window.parent.GetProductId(),
                        OldProductId: OLD_PRODUCT_ID
                    },
                    success: function (response) {
                        var reStr = eval("(" + response.responseText + ")");
                        if (reStr.data != null && PRODUCT_ID == "" && OLD_PRODUCT_ID == "") {
                            window.parent.Is_Continue(reStr.data.Product_Id);
                            //去掉右上角關閉按鈕
                            $(".x-tool-close").hide();
                        }
                        else {
                            Page_Load();
                        }
                        //修改正式数据时运费模式不允许修改（运费模式决定组合商品之规格）
                        if (isEdit == "true") {
                            $("input[name='product_freight']").attr("disabled", true);
                        }
                        //修改正式数据时出貨方式不允许修改（运费模式决定组合商品之规格）
                        if (isEdit == "true") {
                            $("input[name='product_mode']").attr("disabled", true);
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
            beforerender: function () {

            }
        }
    });


    //運送方式
    var product_freight = Ext.get("freight").dom.value;
    Ext.getCmp("product_freight_set").update(product_freight);


    //出貨方式
    var product_mode = Ext.get("mode").dom.value;
    Ext.getCmp("product_mode").update(product_mode);

    $(":radio[name='product_mode']").bind('click', function () {
        if (this.checked && this.value != '2') {
            Ext.Msg.alert(INFORMATION, BAG_CHECK_INFORMATION);
        }
    });

    window.parent.updateAuth(baseInfo, 'colName');

});

function Tomorrow() {
    var d;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate() + 1;                          // 获取日。
    return (new Date(s));                                 // 返回日期。
}


function Page_Load() {
    /**/
    $(".x-tool-close").show();
    Ext.getCmp("baseInfo").getForm().load({
        type: 'ajax',
        url: '/VendorProductCombo/QueryProduct',
        actionMethods: 'post',
        params: {
            "ProductId": PRODUCT_ID,
            OldProductId: OLD_PRODUCT_ID
        },
        success: function (response, opts) {
            var resText = eval("(" + opts.response.responseText + ")");
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

            $("#product_freight" + resText.data.Product_Freight_Set + "").attr("checked", "checked");

            $("#product_mode" + resText.data.Product_Mode + "").attr("checked", "checked");
            $("#tax" + resText.data.Tax_Type + "").attr("checked", "checked");
            ComboStore.load({
                callback: function () {
                    Ext.getCmp('disCombination').setValue(ComboStore.getAt(ComboStore.find("parameterCode", resText.data.Combination)).data.parameterName);
                    //移除單一商品
                    ComboStore.removeAt(ComboStore.find("parameterCode", 1));

                }
            });

            //var brand_name = GetBrand_Name(resText.data.Brand_Id);

            Ext.getCmp("tax_type").setValue(resText.data.Tax_Type);

        }
    })
    /**/
}


//驗證表單
function validateForm(product_start, product_end, expect_time) {
    if (!Ext.getCmp('baseInfo').getForm().isValid()) {
        return;
    }

    if (!Ext.getCmp("brand_id").isValid() || Ext.getCmp("brand_id").getValue() == SELECT) {
        Ext.Msg.alert(PROMPT, BRAND_NOT_NULL);
        return;
    }
    if (!Ext.getCmp("Product_name").isValid()) {
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
    if (Ext.getCmp("combination").getValue() == SELECT) {
        Ext.Msg.alert(PROMPT, COMBINATION_NOT_NULL);
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

    //若填寫預計出貨時間則預計出貨信息必填
    if (Ext.getCmp("e_time").rawValue != "") {
        if (Ext.getCmp("expect_msg").getValue() == "") { Ext.Msg.alert(PROMPT, "請填寫預計出貨信息."); return; }
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

    var retVal = true;
    if (!validateForm(product_start, product_end, expect_time)) {
        window.parent.setMoveEnable(true);
        return false;
    }
    if (!functionid) {
        functionid = '';
    }

    //儲存數據
    Ext.Ajax.request({
        url: '/VendorProductCombo/SaveBaseInfo',
        method: 'POST',
        async: false,
        params: {
            "product_id": Ext.htmlEncode(window.parent.GetProductId()),
            OldProductId: OLD_PRODUCT_ID,
            "brand_id": Ext.htmlEncode(Ext.getCmp("brand_id").getValue()),
            "product_name": Ext.htmlEncode(Ext.getCmp("Product_name").getValue()),
            "product_sort": Ext.htmlEncode(Ext.getCmp("product_sort").getValue()),
            "product_vendor_code": Ext.htmlEncode(Ext.getCmp("product_vendor_code").getValue()),
            "product_start": product_start,
            "product_end": product_end,
            "expect_time": expect_time,
            "product_freight_set": Ext.htmlEncode($(":radio[name='product_freight']:checked")[0].value),
            "product_mode": Ext.htmlEncode($(":radio[name='product_mode']:checked")[0].value),
            "tax_type": Ext.htmlEncode(Ext.getCmp("tax_type").getValue().Tax_Type),
            "combination": Ext.htmlEncode(Ext.getCmp("combination").getValue()),
            "expect_msg": Ext.htmlEncode(Ext.getCmp('expect_msg').getValue())
        }
        , success: function (msg) {
            var resMsg = eval("(" + msg.responseText + ")");
            if (resMsg.success == true && resMsg.msg != null) {
                PRODUCT_ID = resMsg.msg;
                window.parent.SetProductId(PRODUCT_ID);
            }
            if (resMsg.success == true && resMsg.resurt != null && window.parent.GetIsEdit() == "true") {
                Ext.Msg.alert(PROMPT, SAVE_SUCCESS);
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


