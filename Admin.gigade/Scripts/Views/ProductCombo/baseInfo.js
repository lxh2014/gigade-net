var a;
var columnCount_freight;
var columnCount_mode;
var PRODUCT_ID, OLD_PRODUCT_ID = '';
var startDate;

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
        { name: "brand_name", type: "string" }]
});

//品牌store
var brandStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Brand',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Brand/QueryBrand?hideOffGrade=1",
        actionMethods: 'post',
        noCache: false,
        getMethod: function () { return 'get'; },
        reader: {
            type: 'json',
            root: 'item'
        }
    }
});


//tierStore add 2015/03/26
Ext.define("gigade.TierStore", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "schedule_id", type: "int" }
        //, { name: "", type: "string" }
    ]
});

var tierStore = Ext.create('Ext.data.Store', {
    model: 'gigade.TierStore',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/ProductTier/GetTiers",
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});


Ext.onReady(function () {
    PRODUCT_ID = window.parent.GetProductId();
    OLD_PRODUCT_ID = window.parent.GetCopyProductId();

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
        margin: '3 0 0 8',
        defaults: {
            labelWidth: 100,
            padding: '8 0 0 0'
        },
        items: [{
            xtype: 'combobox',
            allowBlank: false,
            hidden: true,
            id: 'brand_id',//品牌
            name: 'Brand_Id',
            store: brandStore,
            colName: 'brand_id',
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
            columnWidth: .70,
            border: 0,
            layout: 'column',
            items: [{
                xtype: 'textfield',
                id: 'Prod_name',//商品名稱
                colName: 'Product_name',
                allowBlank: false,
                hidden: true,
                //disabled: window.parent.GetProductId() != "",//禁用 2014/09/16
                width: 400,
                name: 'Prod_Name',
                fieldLabel: PRODUCT_NAME + '<span style="color:red">*</span>'
            }, {
                xtype: 'textfield',
                margin: '0 0 0 10',
                id: 'prod_sz',//規格欄位
                allowBlank: true,
                maxLength: 10,
                width: 100,
                name: 'Prod_Sz'
            }, {// add by zhuoqin0830w  2015/07/06 添加商品失格欄位
                xtype: 'checkbox',
                margin: '0 0 0 10',
                boxLabel: '失格',
                id: 'off-grade',
                width: 80,
                colName: 'off-grade',
                hidden: true
            }]
        }, {
            xtype: 'combobox',
            allowBlank: false,
            //hidden: true,
            id: 'product_type',//商品型態 add 2014/09/15
            name: 'Product_Type',
            colName: 'product_type',
            store: productTypeStore,
            queryMode: 'local',
            editable: false,
            displayField: 'parameterName',
            valueField: 'parameterCode',
            value: "0",//add 2014/09/19
            emptyText: SELECT,
            fieldLabel: PRODUCT_TYPE + '<span style="color:red">*</span>'
        }, {
            xtype: 'numberfield',
            minValue: 0,
            value: 0,
            hidden: true,
            id: 'product_sort',
            decimalPrecision: 0,
            allowBlank: false,
            fieldLabel: PRODUCT_SORT,
            colName: 'product_sort',
            name: 'Product_Sort'
        }, {
            xtype: 'textfield',
            hidden: true,
            id: 'product_vendor_code',
            fieldLabel: PRODUCT_VENDOR_CODE,
            colName: 'product_vendor_code',
            name: 'Product_Vendor_Code'
        }, {
            xtype: 'panel',
            hidden: true,
            layout: 'hbox',
            id: 'product_start',
            colName: 'product_start',
            allowBlank: false,
            border: false,
            items: [{
                xtype: 'datetimefield',
                disabledMin: true,
                disabledSec: true,
                format: 'Y-m-d H:i:s',
                id: 'p_start',
                value: tomRepeat,
                allowBlank: false,
                fieldLabel: PRODUCT_START,
                //labelWidth: 80,
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
            colName: 'product_end',
            id: 'product_end',
            items: [{
                xtype: 'datetimefield',
                disabledMin: true,
                disabledSec: true,
                format: 'Y-m-d H:i:s',
                id: 'pt_end',
                fieldLabel: PRODUCT_END,
                value: new Date(tomorrow.setFullYear(tomorrow.getFullYear() + 10)),
                allowBlank: false,
                //labelWidth: 80,
                margin: '0 8 0 0',
                listeners: {
                }
            }]
        }, {
            xtype: 'panel',
            hidden: true,
            layout: 'hbox',
            border: false,
            colName: 'expect_time',
            id: 'expect_time',
            items: [{
                xtype: 'datetimefield',
                disabledMin: true,
                disabledSec: true,
                id: 'e_time',
                format: 'Y-m-d H:i:s',
                fieldLabel: EXPECT_TIME,
                //labelWidth: 80,
                margin: '0 8 0 0'
            }, {
                xtype: 'combobox',
                id: 'schedule_id',
                name: 'schedule_id',
                store: tierStore,
                margin: '0 0 0 8 ',
                labelWidth: 60,
                editable: false,
                hidden: true,//PRODUCT_ID == '' ? true : false,
                displayField: 'schedule_id',
                valueField: 'schedule_id',
                emptyText: SELECT,
                fieldLabel: SCHEDULE_SELECT//排程選擇
            }, {
                xtype: 'textfield',
                hidden: true,
                width: 300,
                id: 'expect_msg',
                margin: '0 0 0 8 ',
                //labelWidth: 80,
                fieldLabel: EXPECT_MSG,
                colName: 'expect_msg',
                name: 'expect_msg',
                maxLength: 80//,
                //maxLengthText: '此欄位最多輸入80個字元'
            }]
        }, {
            xtype: 'radiogroup',
            hidden: true,
            id: 'product_freight_set',
            fieldLabel: PRODUCT_FREIGHT_SET,
            colName: 'product_freight_set',
            width: 500,
            name: 'Product_Freight_Set'
        }, {
            xtype: 'radiogroup',
            hidden: true,
            id: 'product_mode',
            colName: 'product_mode',
            name: 'Product_Mode',
            fieldLabel: PRODUCT_MODE,
            margin: '3 11 0 0',
            width: 250
        }, {
            xtype: 'radiogroup',
            hidden: true,
            id: 'tax_type',
            fieldLabel: TAX_TYPE,
            width: 200,
            colName: 'tax_type',//營業稅
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
            id: 'show_in_deliver',//是否顯示於出貨單中 add by Jiajun 2014/09/15
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
            xtype: 'panel',
            border: false,
            hidden: true,
            id: 'comboType',
            colName: 'comboType',
            items: [{
                xtype: 'combobox',
                allowBlank: false,
                hidden: PRODUCT_ID != '',
                id: 'combination',
                name: 'Combination',
                store: ComboStore,
                displayField: 'parameterName',
                valueField: 'parameterCode',
                editable: false,
                queryMode: 'local',
                emptyText: SELECT,
                fieldLabel: COMBINATION + '<span style="color:red">*</span>',
                listeners: {
                    select: function () {
                        Ext.Ajax.request({
                            url: '/ProductCombo/combSpecTempDelete',
                            method: 'post',
                            params: {
                                OldProductId: window.parent.GetCopyProductId()
                            }
                        });
                    }
                }
            }, {
                xtype: 'displayfield',
                width: 500,
                hidden: PRODUCT_ID != '',
                html: '<span style="color:red;font-weight:bold;font-size:11px;font-family:雅黑;float:left">' + ATTENTION + '</span>'
            }, {
                xtype: 'displayfield',
                width: 400,
                hidden: PRODUCT_ID == '',
                fieldLabel: COMBINATION,
                id: 'disCombination'
            }]
        }],
        listeners: {
            beforerender: function () {
                //檢測臨時表中是否有數據
                Ext.Ajax.request({
                    url: '/ProductCombo/QueryProduct',
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

                        var reStr = eval("(" + response.responseText + ")");
                        if (reStr.data != null && PRODUCT_ID == "" && OLD_PRODUCT_ID == "") {
                            window.parent.Is_Continue();
                            //去掉右上角關閉按鈕
                            $(".x-tool-close").hide();
                        }
                        else {
                            Page_Load();
                        }

                        //修改正式数据时运费模式不允许修改（运费模式决定组合商品之规格）
                        if (PRODUCT_ID != "") {
                            $("input[name='product_freight']").attr("disabled", true);
                        }

                        //组合商品出貨方式"自出"設為灰階，不能選擇
                        if (window.parent.GetProductId()) {
                            $("#product_mode1").attr("disabled", true);
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


    //var extra_days = Ext.getCmp("extra_days");
    //出貨方式
    var product_mode = Ext.get("mode").dom.value;
    Ext.getCmp("product_mode").update(product_mode);

    $(":radio[name='product_mode']").bind('click', function () {
        if (this.checked && this.value != '2') {
            Ext.Msg.alert(INFORMATION, BAG_CHECK_INFORMATION);
        }
        ////add by zhuoqin0830w  2015/03/16  根據出貨方式 判斷 是否隱藏 寄倉天數/調度天數 控件 
        //if (this.checked && this.value == '1') {
        //    extra_days.allowBlank = true;
        //    extra_days.hide().reset();
        //} else {
        //    extra_days.allowBlank = false;
        //    extra_days.show().reset();
        //}
        ////add by zhuoqin0830w  2015/03/16  根據出貨方式 判斷 label 顯示 寄倉天數 或 調度天數 
        //if (this.checked && this.value == '3') {
        //    $('#extra_days label').html(CONTROL_EXTRA_DAYS);
        //}
        //if (this.checked && this.value == '2') {
        //    $('#extra_days label').html(SEND_EXTRA_DAYS);
        //}

        window.parent.SetProductMode(this.value);//設置出貨方式 edit by xiangwang0413w 2014/11/13
    });

    window.parent.updateAuth(baseInfo, 'colName');

});



function Tomorrow() {   //edit by jiajun 2014/12/31
    var d;
    d = new Date(); // 创建 Date 对象。 
    d.setDate(d.getDate() + 1);
    return d;
}



function Page_Load() {
    /**/
    $("#off-grade input[type=button]").click(function () {
        var off_grade = Ext.getCmp('off-grade');
        var oldValue = off_grade.getValue();
        Ext.Msg.confirm(ALERT_MESSAGE, AFFIRM_FIX,//提示信息,是否確認修改
        function (button) {
            if (button == 'no') {
                off_grade.setValue(!oldValue);
            }
        });
    });

    $(".x-tool-close").show();
    Ext.getCmp("baseInfo").getForm().load({
        type: 'ajax',
        url: '/ProductCombo/QueryProduct',
        actionMethods: 'post',
        params: {
            "ProductId": PRODUCT_ID,
            OldProductId: OLD_PRODUCT_ID
        },
        success: function (response, opts) {
            var resText = eval("(" + opts.response.responseText + ")");

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
            ////add by zhuoqin0830w  2015/03/16  根據出貨方式 判斷 是否隱藏 寄倉天數/調度天數 控件 
            //if (resText.data.Product_Mode == 1) {
            //    Ext.getCmp("extra_days").hide().allowBlank = true;
            //}
            ////add by zhuoqin0830w  2015/03/16  根據出貨方式 判斷 label 顯示 寄倉天數 或 調度天數 
            //if (resText.data.Product_Mode == 3) {
            //    $('#extra_days label').html(CONTROL_EXTRA_DAYS);
            //}
            //if (resText.data.Product_Mode == 2) {
            //    $('#extra_days label').html(SEND_EXTRA_DAYS);
            //}
            window.parent.SetProductMode(resText.data.Product_Mode); //記錄父商品出貨方式 add by xiangwang0413w 2014/11/14
            if (window.parent.GetProductId() && resText.data.Product_Mode == 1) {//修改時,如果出貨方式為自出商品,則不可設置
                $(":radio[name='product_mode']").attr("disabled", true);
            }

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

            //add 2014/09/16
            Ext.getCmp("show_in_deliver").setValue(resText.data.Show_In_Deliver);//顯示於出貨單中
            Ext.getCmp("process_type").setValue(resText.data.Process_Type);//配送系統
            Ext.getCmp("product_type").setValue(resText.data.Product_Type);//商品形態

            Ext.getCmp("schedule_id").setValue(resText.data.Schedule_Id);//排程ID

            //add 2014/11/18
            Ext.getCmp("prod_sz").setValue(resText.data.Prod_Sz);
            // Ext.getCmp("Prod_name").setValue(resText.data.Prod_Name_Simple);
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
        if (Ext.getCmp("expect_msg").getValue() == "") { Ext.Msg.alert(PROMPT, PLEASE_WRITE_SHIPMENT_MESSAGE); return; }
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
        url: '/ProductCombo/SaveBaseInfo',
        method: 'POST',
        async: false,
        params: {
            "product_id": Ext.htmlEncode(window.parent.GetProductId()),
            OldProductId: OLD_PRODUCT_ID,
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
            "combination": Ext.htmlEncode(Ext.getCmp("combination").getValue()),
            "expect_msg": Ext.htmlEncode(Ext.getCmp('expect_msg').getValue()),

            //add 2014/09/15
            "show_in_deliver": Ext.htmlEncode(Ext.getCmp("show_in_deliver").getValue().Show_In_Deliver),
            "process_type": Ext.htmlEncode(Ext.getCmp("process_type").getValue().Process_Type),
            "product_type": Ext.htmlEncode(Ext.getCmp("product_type").getValue()),

            //add 2015/03/26
            "schedule_id": Ext.htmlEncode(Ext.getCmp("schedule_id").getValue()),

            //add 2014/11/18
            "Prod_Sz": Ext.htmlEncode(Ext.getCmp("prod_sz").getValue()),

            "function": functionid,
            "batch": window.parent.GetBatchNo(),
            //add by zhuoqin0830w 2015/06/25
            "off-grade": Ext.getCmp("off-grade").getValue() ? 1 : 0
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