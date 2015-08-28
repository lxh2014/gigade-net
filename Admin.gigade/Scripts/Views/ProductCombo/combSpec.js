var re = /}{/g;
var PRODUCT_ID;
var isLoad = false;
var comboType;
var combSpecStroe;
var numMustBuy;
var groupNum;
var buyLimit = 0;
var currentRow;
var currentCol;
//edit by mingwei0727w 2015-07-13
var numArray = new Array(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20);
var mustBuyArray = new Array(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
var tempData = null;
var vendorId = null;

Ext.onReady(function () {

    initPanel();

});

//運費模式不匹配
function freightCheck(tempData, resText) {
    switch (tempData.Product_Freight_Set) {
        case 1:
        case 3:
            if (resText.data.Product_Freight_Set != 1 && resText.data.Product_Freight_Set != 3) {
                Ext.Msg.alert(INFORMATION, PRODUCT_FREIGTH_NOT_MATCH); return false;
            }; break;
        case 2:
        case 4: if (resText.data.Product_Freight_Set != 2 && resText.data.Product_Freight_Set != 4) {
            Ext.Msg.alert(INFORMATION, PRODUCT_FREIGTH_NOT_MATCH); return false;
        }; break;
        case 5:
        case 6: if (resText.data.Product_Freight_Set != 5 && resText.data.Product_Freight_Set != 6) {
            Ext.Msg.alert(INFORMATION, PRODUCT_FREIGTH_NOT_MATCH); return false;
        }; break;
        default: return false; break;

    }
    return true;
}


//输入商品编号后判断商品是否符合要求
function productCheck(resText,type) {
    if (resText.data.Combination != 1) {
        Ext.Msg.alert(INFORMATION, INPUT_MUSTBE_SINGLE);
        combSpecStroe.getAt(currentRow).set('Product_Name', '');
        return;
    }
    if (resText.data.Product_Status == 0 || resText.data.Product_Status == 1) {
        Ext.Msg.alert(INFORMATION, INPUT_PRODUCT_STATUS);
        combSpecStroe.getAt(currentRow).set('Product_Name', '');
        return;
    }

    //根據傳來的 type 屬性判斷組合的方式然後 判斷輸入的子商品編號是否存在   add by zhuoqin0830w 2015/02/12
    if (type == FIXED_COMBO) {
        if (combSpecStroe.data.length != 1) {
            for (var i = 0; i < combSpecStroe.data.length - 1; i++) {
                if (resText.data.Product_Id == combSpecStroe.getAt(i).data.Child_Id) {
                    Ext.Msg.alert(INFORMATION, CHILD_PRODUCT_SAME);
                    combSpecStroe.getAt(currentRow).set('Child_Id', '');
                    return;
                }
            }
        }
    } else if (type == OPTIONAL_COMBO) {
        var s = 0;
        for (var i = 0; i < combSpecStroe.data.length; i++) {
            if (resText.data.Product_Id == combSpecStroe.getAt(i).data.Child_Id) {
                s++;
            }
        }
        if (s > 1) {
            Ext.Msg.alert(INFORMATION, CHILD_PRODUCT_SAME);
            combSpecStroe.getAt(currentRow).set('Child_Id', '');
            return;
        }
    }

    //自出組合商品之子商品需為自出商品且為同一供應商 edit by xiangwang0413w 2014/11/14
    if (window.parent.GetProductMode() == 1) {//product_mode=1(父商品為自出商品)
        if (resText.data.Product_Id == combSpecStroe.getAt(0).data.Child_Id) {
            vendorId = resText.data.Vendor_Id;//記錄第一行商品的供應商ID
        }

        if (resText.data.Product_Mode != 1) {//自出商品之子商品需為自出商品
            Ext.Msg.alert(INFORMATION, CHILD_PRODUCT_SELF);
            combSpecStroe.getAt(currentRow).set('Product_Name', '');
            return;
        }
        if (resText.data.Vendor_Id != vendorId) {//自出商品之子商品需為同一供應商
            Ext.Msg.alert(INFORMATION, CHILD_PRODUCT_SAME_VENDOR);
            combSpecStroe.getAt(currentRow).set('Product_Name', '');
            return;
        }
    }
    else if (resText.data.Product_Mode == 1) {//原先邏輯
        Ext.Msg.alert(INFORMATION, PRODUCT_MODE_SELF);
        combSpecStroe.getAt(currentRow).set('Product_Name', '');
        return;
    }



    //检查运费模式
    if (freightCheck(tempData, resText)) {
        combSpecStroe.getAt(currentRow).set('Product_Name', resText.data.Product_Name);
    }
    else {
        combSpecStroe.getAt(currentRow).set('Product_Name', '');
    }
}

function initPanel() {
    PRODUCT_ID = window.parent.GetProductId();

    Ext.Ajax.request({
        url: '/ProductCombo/QueryProduct',
        method: 'POST',
        params: {
            ProductId: PRODUCT_ID,
            OldProductId: window.parent.GetCopyProductId()
        },
        success: function (response, opts) {
            var resText = eval("(" + response.responseText + ")");
            if (resText != null && resText.data != null) {
                comboType = resText.data.Combination;
                tempData = resText.data;
                switch (comboType) {
                    case 2: createComboFixed(); break;
                    case 3: createComboOptional(); break;
                    case 4: createComboGroup(); break;
                    default: break;
                }
            }
        },
        failure: function (response, opts) {
            return false;
        }
    });
}


function save() {
    if (PRODUCT_ID != '') {
        Ext.Msg.alert(INFORMATION, COMBO_SPEC_UNCHANGE);
        return;
    }
    var saveResult;
    switch (comboType) {
        case 2: saveResult = fixedSave(); break;
        case 3: saveResult = optionalSave(); break;
        case 4: saveResult = groupSave(); break;
        default: break;

    }
    return saveResult;
}


function resTextGet(product_id) {
    var result;
    Ext.Ajax.request({
        url: '/ProductCombo/QueryProduct',
        method: 'POST',
        async: false,
        params: {
            ProductId: product_id
        },
        success: function (response, opts) {
            result = eval("(" + response.responseText + ")");
        },
        failure: function (response, opts) {
            return false;
        }
    });
    return result;
}

/* 固定組合保存*/
function fixedSave() {
    var result = true;
    var tempStr = "[";
    for (var i = 0, j = combSpecStroe.getCount() ; i < j; i++) {
        var data = combSpecStroe.getAt(i).data;
        if (data.Product_Name) {
            tempStr += '{child_id:"' + data.Child_Id + '",s_must_buy:' + data.S_Must_Buy + '}';
        }
        else {
            result = false;
            break;
        }

        //保存时再次验证子商品运费模式是否与组合商品一致
        var resText = resTextGet(data.Child_Id);
        if (!freightCheck(tempData, resText)) {
            combSpecStroe.getAt(i).set("Product_Name", "");
            return;
        }
    }
    tempStr = tempStr.replace(re, "},{");
    tempStr += "]";

    if (!result) {
        Ext.Msg.alert(NOTICE, COMPLETE);
        return result;
    }

    Ext.Ajax.request({
        url: '/ProductCombo/combSpecSave',
        method: 'POST',
        async: false,
        params: {
            'ProductId': PRODUCT_ID,
            'OldProductId': window.parent.GetCopyProductId(),
            'isLoad': isLoad,
            'resultStr': tempStr
        },
        success: function (response, opts) {
            var resText = eval("(" + response.responseText + ")");
            if (resText.success) {
                if (PRODUCT_ID != '') {
                    Ext.Msg.alert(NOTICE, SAVE_SUCCESS);
                }
            }
            else {
                result = false;
                Ext.Msg.alert(NOTICE, SAVE_FAIL);
                window.parent.setMoveEnable(true);
            }
        },
        failure: function (response, opts) {
            Ext.Msg.alert(NOTICE, SAVE_FAIL);
            window.parent.setMoveEnable(true);
            result = false;
        }
    });
    return result;
}

/*任意組合保存*/
function optionalSave() {
    if (!Ext.getCmp('optionalPanel').getForm().isValid()) {
        return false;
    }

    var msg;
    var result = true;
    var tempStr = "[";
    if (combSpecStroe.getCount() <= 0) {
        Ext.Msg.alert(NOTICE, COMPLETE);
        result = false;
        return false;
    }
    else {
        var sum = 0;
        var isAllMustBuy = true;    //判斷當前子商品的必購數量是否全不為0
        for (var i = 0, j = combSpecStroe.getCount() ; i < j; i++) {
            var data = combSpecStroe.getAt(i).data;
            if (data.Product_Name) {
                var limit = Ext.getCmp('chkNum').getValue() ? 1 : 0;
                tempStr += '{child_id:"' + data.Child_Id + '",s_must_buy:' + data.S_Must_Buy + ',g_must_buy:' + numMustBuy + ',buy_limit:' + limit + '}';
                sum += parseInt(data.S_Must_Buy);
                if (data.S_Must_Buy <= 0) {
                    isAllMustBuy = false;
                }
            }
            else {
                result = false;
                Ext.Msg.alert(NOTICE, COMPLETE);
                return false;
            }

            //保存时再次验证子商品运费模式是否与组合商品一致
            var resText = resTextGet(data.Child_Id);
            if (!freightCheck(tempData, resText)) {
                combSpecStroe.getAt(i).set("Product_Name", "");
                return;
            }
        }
        if (sum > numMustBuy) {
            result = false;
            msg = THIS_COMBO_JUST_SELECT + numMustBuy + UNITS_PRODUCT;//該組合中最多只能選擇,件商品~~;
            Ext.Msg.alert(NOTICE, msg);
            return false;
        }
        if (isAllMustBuy && sum != numMustBuy) {
            result = false;
            msg = CHILD_PRODUCT_BUY_IS_DIFFERENT_WITH_SELECTED;//子商品的必購數量總值與組合的需選擇數量不符~~;
            Ext.Msg.alert(NOTICE, msg);
            return false;
        }


        tempStr = tempStr.replace(re, "},{");
        tempStr += "]";
    }

    if (!result) {
        Ext.Msg.alert(NOTICE, msg);
        return result;
    }
    Ext.Ajax.request({
        url: '/ProductCombo/combSpecSave',
        method: 'POST',
        async: false,
        params: {
            'ProductId': PRODUCT_ID,
            'OldProductId': window.parent.GetCopyProductId(),
            'isLoad': isLoad,
            'resultStr': tempStr
        },
        success: function (response, opts) {
            var resText = eval("(" + response.responseText + ")");
            if (resText.success) {
                if (PRODUCT_ID != '') {
                    Ext.Msg.alert(NOTICE, SAVE_SUCCESS);
                }
            }
            else {
                Ext.Msg.alert(NOTICE, SAVE_FAIL);
                result = false;
                window.parent.setMoveEnable(true);
            }
        },
        failure: function (response, opts) {
            Ext.Msg.alert(NOTICE, SAVE_FAIL);
            window.parent.setMoveEnable(true);
            result = false;
        }
    });
    return result;
}

/*群組搭配保存*/
function groupSave() {
    if (!Ext.getCmp('topPanel').getForm().isValid()) {
        return false;
    }
    var result = true;
    var tempStr = "[";
    var msg;
    if (groupNum > 0) {

        for (var i = 1; i <= groupNum; i++) {
            if (!Ext.getCmp('G_' + i)) {
                Ext.Msg.alert(NOTICE, COMPLETE);
                return false;
            }
            var sum = 0;
            // var mustId = isLoad ? 'disMustBuy_' + i : 'M_' + i;
            var mustId = 'M_' + i;
            var mustBuyNum = Ext.getCmp(mustId).getValue();
            var limit = Ext.getCmp('chkNum').getValue() ? 1 : 0;
            for (var m = 0, n = Ext.getCmp('G_' + i).getStore().getCount() ; m < n; m++) {
                var data = Ext.getCmp('G_' + i).getStore().getAt(m).data;
                if (data.Product_Name) {

                    tempStr += '{child_id:"' + data.Child_Id + '",s_must_buy:' + data.S_Must_Buy + ',g_must_buy:' + Ext.getCmp(mustId).getValue() + ',pile_id:' + i + ',buy_limit:' + limit + '}';
                    if (data.S_Must_Buy != 0) {
                        sum += parseInt(data.S_Must_Buy);
                    }

                }
                else {
                    Ext.Msg.alert(NOTICE, Ext.String.format(INPUT_GROUP_INFO, i));
                    return false;
                }

                //保存时再次验证子商品运费模式是否与组合商品一致
                var resText = resTextGet(data.Child_Id);
                if (!freightCheck(tempData, resText)) {
                    Ext.getCmp('G_' + i).getStore().getAt(m).set("Product_Name", "");
                    return;
                }
            }
            if (sum > mustBuyNum) {
                Ext.Msg.alert(NOTICE, GROUP + i + AT_MOST_SELECT + mustBuyNum + UNITS_PRODUCT);//群組' + i + '最多只能選擇' + mustBuyNum + '件商品~~
                return false;
            }
        }

        tempStr = tempStr.replace(re, "},{");
        tempStr += "]";
    }
    else {
        Ext.Msg.alert(NOTICE, COMPLETE);
        return false;
    }

    Ext.Ajax.request({
        url: '/ProductCombo/combSpecSave',
        method: 'POST',
        async: false,
        params: {
            'ProductId': PRODUCT_ID,
            'OldProductId': window.parent.GetCopyProductId(),
            'isLoad': isLoad,
            'resultStr': tempStr
        },
        success: function (response, opts) {
            var resText = eval("(" + response.responseText + ")");
            if (resText.success) {
                if (PRODUCT_ID != '') {
                    Ext.Msg.alert(NOTICE, SAVE_SUCCESS);
                }
            }
            else {
                Ext.Msg.alert(NOTICE, SAVE_FAIL);
                result = false;
                window.parent.setMoveEnable(true);
            }
        },
        failure: function (response, opts) {
            Ext.Msg.alert(NOTICE, SAVE_FAIL);
            window.parent.setMoveEnable(true);
            result = false;
        }
    });
    return result;
}