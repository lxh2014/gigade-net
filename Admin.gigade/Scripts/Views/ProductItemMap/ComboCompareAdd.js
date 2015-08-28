
var comboGrid;
var columnCount;
var mustBuyArray = new Array(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
var comboData = new Array(1, 2, 3);
var currentCol = 0;
var currentRow = 0;
var c_get_data;
var setNumbl = false; //是否存在必選單品
var Hasbl = false; //存在為false
var oldItem = "";

function showComboAdd(get_data) {

    c_get_data = get_data;

    //組合商品子商品之規格
    var arry = get_data.strSpec.split(',');

    //任意組合商品之組成商需為無規格
    if (get_data.combination == 3) {
        var arry = get_data.strSpec.split(',');
        if (arry.length > 0) {
            for (var i = 0, j = arry.length; i < j; i++) {
                if (arry[i] != 0) {
                    Ext.Msg.alert(INFORMATION, OPTIONAL_MUST_NULL_SPEC);
                    return;
                }
            }
        }
    }
    //排除各自定價商品
    if(get_data.price_type == 2){
        Ext.Msg.alert(INFORMATION, PRICE_TYPE_NO2);
        return;
    }
    //群組搭配商品之組成商品需為無規格，且限定「每種一單位」 每個群組之product_combo. g_must_buy為1
    if (get_data.combination == 4) {
        var arry = get_data.strSpec.split(',');
        if (arry.length > 0) {
            for (var i = 0, j = arry.length; i < j; i++) {
                if (arry[i] != 0) {
                    Ext.Msg.alert(INFORMATION, GROUP_MUST_NULL_SPEC);
                    return;
                }
            }
        }

        var gmustArry = get_data.g_must_buy.split(',');
        if (gmustArry.length > 0) {
            for (var i = 0, j = gmustArry.length; i < j; i++) {
                if (gmustArry[i] != 1) {
                    Ext.Msg.alert(INFORMATION, GROUP_G_MUST_BUY_1);
                    return;
                }
            }
        }

        if (get_data.buy_limit != 1) {
            Ext.Msg.alert(INFORMATION, GROUP_BUY_LIMIT_1);
            return;
        }

    }



    //添加 Store fields
    var fields = eval(get_data.items);
    fields.push({ name: 'rid', type: 'int' });
    fields.push({ name: 'group_item_id', type: 'string' });
    fields.push({ name: 'product_name', type: 'string' });
    fields.push({ name: 'channel_detail_id', type: 'string' });
    fields.push({ name: 'product_cost', type: 'int' });
    fields.push({ name: 'product_price', type: 'int' });

    columnCount = get_data.count;
    var DoUrl = "/ProductItemMap/FixedQuery";
    //僅限一單位勾選了就用g_must_buy
    //    if (get_data.buy_limit != 0) {
    //        columnCount = get_data.g_must_buy;
    //    }
    if (get_data.combination == 3) {
        DoUrl = "/ProductItemMap/OptionalQuery";
        Ext.Ajax.request({
            url: DoUrl,
            params: {
                cId: Ext.htmlEncode(Ext.getCmp("comboxOutSite").getValue()),
                ProductId: Ext.htmlEncode(Ext.getCmp("txtProductId").getValue()),
                pmId: Ext.htmlEncode(Ext.getCmp("comboxSitePrice").getValue()),//edit by xiangwang0413w 2014/07/20 根根據站台價格查詢商品對照
                type: 'searchInfo'
            },
            success: function (response) {
                oldItem = response.responseText.split('data:')[1].substring(0, response.responseText.split('data:')[1].length - 1);
            }
        });
    }
    if (get_data.combination == 4) {
        DoUrl = "/ProductItemMap/GroupQuery";
    }

    productStore = Ext.create('Ext.data.Store', {
        fields: fields,
        pageSize: pageSize,
        sorters: [{
            property: 'rid',
            direction: 'DESC'
        }],
        proxy: {
            type: 'ajax',
            url: DoUrl,
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });


    if (get_data.combination == 3 && get_data.buy_limit != 1) {
        var states;
        optionalShow(get_data);
    }
    else {
        fixedgroupShow(get_data);
    }





}
//判斷是否需要移除數據
function showExistData(productStore) {
    var bool = false;
    for (var i = 0, j = productStore.getCount(); i < j; i++) {
        if (productStore.getAt(i).data.rid) {
            bool = true;
        }
    }
    //是否已經建立過對照
    if (bool) {
        for (var i = 0; i < productStore.getCount(); i++) {
            if (!productStore.getAt(i).data.rid) {
                productStore.removeAt(i);
                --i;
            }
        }
    }
    else {
        for (var i = 0, j = productStore.getCount(); i < j; i++) {
            if (bool == false) {
                if (setNumbl == false) {//不存在必選單品就留下一筆記錄
                    productStore.removeAt(1);
                }
            }
        }
    }

}

//固定及群組 Grid
function fixedgroupShow(get_data) {

    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
    myMask.show();

    //var columnStr = '[{ header: NO, xtype: \'rownumberer\', width: 50 }';
    //for (var i = 1; i <= columnCount; i++) {
    //    columnStr += Ext.String.format(',{ header: \'{0}' + i + '\', dataIndex: \'product_name_' + i + '\', width: 150 }', COMBO_PRODUCT);
    //    columnStr += Ext.String.format(',{ header: \'{0}' + i + '\', dataIndex: \'product_num_' + i + '\', width: 80 }', COMBO_NUM);
    //}
    //columnStr += "]";
   // var columns = Ext.decode(columnStr);
    //edit by xiangwang0413w 2014/07/11 調整顯示順序，可編輯的列顯示到前面
    var columns = [{ header: NO, xtype: 'rownumberer', width: 50 }];
    columns.push({ header: OUTSITE_PRODUCT_NAME, dataIndex: 'product_name', width: 200, editor: { xtype: 'textfield', allowBlank: false} });
    columns.push({ header: OUTSITE_PRODUCT_ID, dataIndex: 'channel_detail_id', width: 130, editor: { xtype: 'textfield', allowBlank: false} });
    columns.push({ header: OUTSITE_PRODUCT_COST, dataIndex: 'product_cost', hidden: true, width: 120, editor: { xtype: 'numberfield', decimalPrecision: 0, minValue: 0, allowBlank: false} });
    columns.push({ header: OUTSITE_PRODUCT_PRICE, dataIndex: 'product_price', width: 120, editor: { xtype: 'numberfield', decimalPrecision: 0, minValue: 0, allowBlank: false } });
    for (var i = 1; i <= columnCount; i++) {
        columns.push(Ext.decode(Ext.String.format('{ header: \'{0}' + i + '\', dataIndex: \'product_name_' + i + '\', width: 150 }', COMBO_PRODUCT)));
        columns.push(Ext.decode(Ext.String.format('{ header: \'{0}' + i + '\', dataIndex: \'product_num_' + i + '\', width: 80 }', COMBO_NUM)));
    }
    columns.push({ header: BTN_DELETE, width: 50, xtype: 'actioncolumn', items: [{ icon: '../../../Content/img/icons/delete.gif', handler: function (grid, rowIndex, colIndex) { comboDelete(rowIndex); } }] });

    var fixedCellediting = Ext.create('Ext.grid.plugin.CellEditing', {
        clicksToEdit: 1
    });

    comboGrid = Ext.create('Ext.grid.Panel', {
        id: 'comboGrid',
        autoScroll: true,
        store: productStore,
        columns: columns,
        frame: true,
        plugins: [fixedCellediting],
        dockedItems: [{
            xtype: 'panel',
            layout: 'hbox',
            plain: true,
            bodyStyle: 'padding:3px',
            items: [{
                xtype: 'displayfield',
                labelWidth: 65,
                margin: '0 15 0 0',
                fieldLabel: PRODCUT_ID,
                id: 'txtpNo'

            }, {
                xtype: 'displayfield',
                labelWidth: 65,
                margin: '0 15 0 0',
                fieldLabel: PRODUCT_NAME,
                id: 'txtpName'

            }, {
                xtype: 'displayfield',
                labelWidth: 65,
                margin: '0 15 0 0',
                fieldLabel: PRODUCT_COST,
                id: 'txtCost'
            }, {
                xtype: 'displayfield',
                labelWidth: 65,
                margin: '0 15 0 0',
                fieldLabel: PRODUCT_PRICE,
                id: 'txtPrice'
            }, {
                xtype: 'button',
                width: 100,
                text: COPY,
                handler: function () {
                    comboCopy(get_data);
                }
            }]
        }],
        buttons: [{
            text: SAVE,
            handler: function () {
                fixedCellediting.completeEdit();
                if (productStore.getCount() > 0) {
                    var bool = false;

                    //驗證信息是否填寫完整
                    for (var i = 0, j = productStore.getCount(); i < j; i++) {
                        var pd = productStore.getAt(i).data;
                        if (pd.product_name == '' || pd.channel_detail_id == '') {
                            Ext.Msg.alert(INFORMATION, COMPLETE_MAP_INFO);
                            return;
                        }
                    }

                    for (var i = 0, j = productStore.getCount(); i < j; i++) {
                        var pd = productStore.getAt(i).data;
                        //驗證外站商品售價是否低於原商品售價
                        if (parseFloat(pd.product_price) < parseFloat(get_data.price)) {
                            bool = true;
                            break;
                        }
                    }

                    if (bool) {
                        Ext.Msg.confirm(INFORMATION, CHANNEL_PRICE_LOWER, function (btn) {
                            if (btn == 'yes') {
                                comboSave(get_data);
                            }
                        });
                    }
                    else {
                        comboSave(get_data);
                    }
                }
                else {
                    Ext.Msg.alert(INFORMATION, NULL_DATA_SAVE);
                    return;
                }
            }
        }]
    });

    productStore.load({
        params: {
            cId: Ext.htmlEncode(Ext.getCmp("comboxOutSite").getValue()),
            ProductId: Ext.htmlEncode(get_data.parent_id),
            pmId: Ext.htmlEncode(Ext.getCmp("comboxSitePrice").getValue() == null ? 0 : Ext.getCmp("comboxSitePrice").getValue()),//edit by xiangwang0413w 2014/07/20 根根據站台價格查詢商品對照
        },
        callback: function (records) {
            myMask.hide();
            if (get_data.buy_limit == 1) {
                for (var i = 0, j = records.length; i < j; i++) {
                    for (var m = 1; m <= columnCount; m++) {
                        productStore.getAt(i).set('product_num_' + m, 1);
                    }
                }
            }
        }
    });

    Ext.getCmp('txtpNo').setValue(get_data.parent_id);
    Ext.getCmp('txtpName').setValue(get_data.product_name);
    Ext.getCmp('txtCost').setValue(get_data.cost);
    Ext.getCmp('txtPrice').setValue(get_data.price);

    addGrid = comboGrid;
    Ext.getCmp('addWin').add(addGrid);
}


//任選 Grid
function optionalShow(get_data) {
    // var columnStr = '[{ header: NO, xtype: \'rownumberer\', width: 50 }';
    //edit by xiangwang0413w 2014/07/11 調整顯示順序，可編輯的列顯示到前面
    var columns = [{ header: NO, xtype: 'rownumberer', width: 50 }];
    columns.push({ header: OUTSITE_PRODUCT_NAME, dataIndex: 'product_name', width: 200, editor: { xtype: 'textfield', allowBlank: false} });
    columns.push({ header: OUTSITE_PRODUCT_ID, dataIndex: 'channel_detail_id', width: 130, editor: { xtype: 'textfield', allowBlank: false} });
    columns.push({ header: OUTSITE_PRODUCT_COST, dataIndex: 'product_cost', hidden: true, width: 120, editor: { xtype: 'numberfield', decimalPrecision: 0, minValue: 0, allowBlank: false} });
    columns.push({ header: OUTSITE_PRODUCT_PRICE, dataIndex: 'product_price', width: 120, editor: { xtype: 'numberfield', decimalPrecision: 0, minValue: 0, allowBlank: false} });
    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
    myMask.show();


    productStore.load({
        params: {
            cId: Ext.htmlEncode(Ext.getCmp("comboxOutSite").getValue()),
            pmId: Ext.htmlEncode(Ext.getCmp("comboxSitePrice").getValue()),//edit by xiangwang0413w 2014/07/20 根根據站台價格查詢商品對照
            ProductId: Ext.htmlEncode(get_data.parent_id)
        },
        callback: function (records, operation, success) {

            myMask.hide();

            //判斷是否存在必選單品 
            setNumbl = false;
            if (records.length == 0) { Ext.Msg.alert(INFORMATION, DATA_LOAD_FAILURE); return; }
            for (var i = 1, j = columnCount; i <= j; i++) {
                if (eval('records[0].data.s_must_buy_' + i + '') != 0) {
                    setNumbl = true; //存在為true
                    break;
                }
            }

            //
            for (var i = 1; i <= columnCount; i++) {

                get_data.parent_id
                states = Ext.create('Ext.data.Store', {
                    id: 'store_' + i + '',
                    fields: ['product_name', 'item_id'],
                    proxy: {
                        type: 'ajax',
                        url: '/ProductItemMap/QueryComboItem',
                        async: false,
                        actionMethods: 'post',
                        reader: {
                            type: 'json',
                            root: 'data'
                        }
                    }
                });



                states.load({ params: { ParentId: get_data.parent_id } });
                //edit by xiangwang0413w 2014/07/11 調整顯示順序，可編輯的列顯示到前面
                columns.push(Ext.decode(Ext.String.format('{ header: \'{0}' + i + '\', dataIndex: \'product_name_' + i + '\', width: 150}', COMBO_PRODUCT)));
                columns.push(Ext.decode(Ext.String.format('{ header: \'{0}' + i + '\', dataIndex: \'product_num_' + i + '\', width: 80, editor: { xtype: \'combobox\',store:mustBuyArray, allowBlank: false} }', COMBO_NUM)));
                //columnStr += Ext.String.format(',{ header: \'{0}' + i + '\', dataIndex: \'product_name_' + i + '\', width: 150}', COMBO_PRODUCT);
                //columnStr += Ext.String.format(',{ header: \'{0}' + i + '\', dataIndex: \'product_num_' + i + '\', width: 80, editor: { xtype: \'combobox\',store:mustBuyArray, allowBlank: false} }', COMBO_NUM);

                //columnStr += Ext.String.format(',{ header: \'{0}' + i + '\', dataIndex: \'product_name_' + i + '\', width: 150, editor: { xtype: \'combobox\',store:states,queryMode: \'local\',displayField: \'product_name\',valueField: \'product_name\',allowBlank: false', COMBO_PRODUCT);
                //columnStr += ',listeners:{select:function(e,records){productStore.getAt(currentRow).set("product_name_' + i + '",records[0].data.product_name);productStore.getAt(currentRow).set("item_id_' + i + '",records[0].data.item_id)}} } }';
                //columnStr += Ext.String.format(',{ header: \'{0}' + i + '\', dataIndex: \'product_num_' + i + '\', width: 80, editor: { xtype: \'combobox\',store:mustBuyArray, allowBlank: false} }', COMBO_NUM);

            }
            //columnStr += "]";

            //如果有一筆對照存在數據庫,則只顯示有庫中數據
            showExistData(productStore);


            //var columns = Ext.decode(columnStr);
            //columns.push({ header: OUTSITE_PRODUCT_NAME, dataIndex: 'product_name', width: 200, editor: { xtype: 'textfield', allowBlank: false} });
            //columns.push({ header: OUTSITE_PRODUCT_ID, dataIndex: 'channel_detail_id', width: 130, editor: { xtype: 'textfield', allowBlank: false} });
            //columns.push({ header: OUTSITE_PRODUCT_COST, dataIndex: 'product_cost', hidden: true, width: 120, editor: { xtype: 'numberfield', decimalPrecision: 0, minValue: 0, allowBlank: false} });
            //columns.push({ header: OUTSITE_PRODUCT_PRICE, dataIndex: 'product_price', width: 120, editor: { xtype: 'numberfield', decimalPrecision: 0, minValue: 0, allowBlank: false} });
            columns.push({ header: BTN_DELETE, width: 50, xtype: 'actioncolumn', items: [{ icon: '../../../Content/img/icons/delete.gif', handler: function (grid, rowIndex, colIndex) { comboDelete(rowIndex); } }] });

            var optionalCellediting = Ext.create('Ext.grid.plugin.CellEditing', {
                clicksToEdit: 1,
                listeners: {
                    beforeedit: function (e) {
                        currentCol = e.colIdx; //一列
                        currentRow = e.rowIdx; //一行
                        if (currentCol > 4) {//edit by xiangwang0413w 2014/07/11 如果當前列小於4，就可編輯
                            var currentdata = e.record.data; //數據
                            var current =currentCol- 4;// 當前組合商品數 edit by xiangwang0413w 2014/07/11
                            var i = current % 2 == 0 ? current / 2 : parseInt(current / 2) + 1; //5 就是3 6就是3
                            //var IsModefiled = e.record.isModified('product_num_' + i + ''); //這一列是否已經編輯過
                            var ControlValue = eval('currentdata.s_must_buy_' + i + '');
                            if (current >= columnCount * 2) {
                                ControlValue = 0;
                            }
                            if (setNumbl == true) {//存在必選單品
                                if (ControlValue != 0) {//有必選的商品名稱和數量不可以該
                                    return false;
                                }
                            }
                        }
                    }
                }
            });

            comboGrid = Ext.create('Ext.grid.Panel', {
                id: 'comboGrid',
                autoScroll: true,
                store: productStore,
                columns: columns,
                frame: true,
                plugins: [optionalCellediting],
                tbar: [{ xtype: 'button',
                    //hidden: setNumbl,
                    text: ADD,
                    iconCls: 'ui-icon ui-icon-add',
                    handler: function () {
                        productStore.add(eval(oldItem));
                    }
                }],
                dockedItems: [{
                    xtype: 'panel',
                    layout: 'hbox',
                    plain: true,
                    bodyStyle: 'padding:3px',
                    items: [{
                        xtype: 'displayfield',
                        labelWidth: 65,
                        margin: '0 15 0 0',
                        fieldLabel: PRODCUT_ID,
                        id: 'txtpNo'
                    }, {
                        xtype: 'displayfield',
                        labelWidth: 65,
                        margin: '0 15 0 0',
                        fieldLabel: PRODUCT_NAME,
                        id: 'txtpName'

                    }, {
                        xtype: 'displayfield',
                        labelWidth: 65,
                        margin: '0 15 0 0',
                        fieldLabel: PRODUCT_COST,
                        id: 'txtCost'
                    }, {
                        xtype: 'displayfield',
                        labelWidth: 65,
                        margin: '0 15 0 0',
                        fieldLabel: PRODUCT_PRICE,
                        id: 'txtPrice'
                    }, {
                        xtype: 'button',
                        width: 100,
                        text: COPY,
                        handler: function () {
                            comboCopy(get_data);
                        }
                    }]
                }],
                buttons: [{
                    text: SAVE,
                    handler: function () {
                        optionalCellediting.completeEdit();
                        if (productStore.getCount() > 0) {
                            var bool = false;
                            //判斷對照之需選單位數量是否正確
                            for (var i = 0, j = productStore.getCount(); i < j; i++) {
                                var s_must_buy_count = 0;
                                for (var l = 0, m = columnCount; l < m; l++) {
                                    if (eval('productStore.getAt(i).data.product_name_' + (l + 1) + '') != "") {
                                        s_must_buy_count = parseInt(s_must_buy_count) + parseInt(eval('productStore.getAt(i).data.product_num_' + (l + 1) + ''));
                                    }
                                }
                                if (s_must_buy_count != get_data.g_must_buy) {
                                    Ext.Msg.alert(INFORMATION, Ext.String.format(G_MUST_BUY_INFO, i + 1, get_data.g_must_buy));
                                    return;
                                }
                            }
                            //驗證信息是否填寫完整
                            for (var i = 0, j = productStore.getCount(); i < j; i++) {
                                var pd = productStore.getAt(i).data;
                                if (pd.product_name == '' || pd.channel_detail_id == '') {
                                    Ext.Msg.alert(INFORMATION, COMPLETE_MAP_INFO);
                                    return;
                                }
                            }

                            //驗證同一個對照中是否存在重複商品
                            for (var i = 0, j = productStore.getCount(); i < j; i++) {
                                var pd = productStore.getAt(i).data;
                                pd.group_item_id = '';
                                for (var m = 1; m <= columnCount; m++) {
                                    var current_item_id = eval('pd.item_id_' + m + '');
                                    for (var n = 1; n <= columnCount; n++) {
                                        //0 表示商品名稱沒有選擇
                                        if (n != m && current_item_id == eval('productStore.getAt(i).data.item_id_' + n + '')) {
                                            Ext.Msg.alert(INFORMATION, Ext.String.format(OPTIONAL_ITEM_REPEAT, (i + 1)));
                                            return;
                                        }
                                    }

                                    if (m > 1) {
                                        pd.group_item_id += ",";
                                    }
                                    pd.group_item_id += current_item_id;
                                }
                                pd.group_item_id = orderAsc(pd.group_item_id);   //將組合下的item_id集排序

                                //驗證外站商品售價是否低於原商品售價
                                if (parseFloat(pd.product_price) < parseFloat(get_data.price)) {
                                    bool = true;
                                }

                            }

                            for (var i = 0, j = productStore.getCount(); i < j; i++) {
                                var pd = productStore.getAt(i).data;
                                //驗證一個 組合 中是否存在重複商品
                                for (var k = 0; k < j; k++) {
                                    if (i != k && pd.group_item_id == productStore.getAt(k).data.group_item_id) {//首先判斷這個對照中是否有對照的所有商品相同 add by hufeng0813w 2014/03/31
                                        //再進來判斷所有商品對照的數量是否全部相等 例如 pd.item_id_1==productStore.getAt(k).data.item_id_3 就看他們的數量是否也相等 add by hufeng0813w 2014/03/31
                                        var SameCount = 0; //相同的數量
                                        for (var n = 0, m = columnCount; n < m; n++) {
                                            for (var l = 0; l < m; l++) {
                                                //判斷商品item_id 和商品的數量是否完全一致
                                                if ((eval('pd.item_id_' + (n + 1) + '') == eval('productStore.getAt(k).data.item_id_' + (l + 1) + '')) && (eval('pd.product_num_' + (n + 1) + '') == eval('productStore.getAt(k).data.product_num_' + (l + 1) + ''))) {
                                                    SameCount++;
                                                }
                                            }
                                            if (SameCount == 0) {//沒有一個商品相同結束循環
                                                break;
                                            }
                                        }
                                        if (SameCount == columnCount) {
                                            Ext.Msg.alert(INFORMATION, Ext.String.format(OPTIONAL_MAP_REPEAT, k + 1, i + 1));
                                            return;
                                        }
                                    }
                                }
                            }

                            if (bool) {
                                Ext.Msg.confirm(INFORMATION, CHANNEL_PRICE_LOWER, function (btn) {
                                    if (btn == 'yes') {
                                        comboSave(get_data);
                                    }
                                });
                            }
                            else {
                                comboSave(get_data);
                            }


                        }
                        else {
                            Ext.Msg.alert(INFORMATION, NULL_DATA_SAVE);
                            return;
                        }
                    }
                }]
            });
            Ext.getCmp('txtpNo').setValue(get_data.parent_id);
            Ext.getCmp('txtpName').setValue(get_data.product_name);
            Ext.getCmp('txtCost').setValue(get_data.cost);
            Ext.getCmp('txtPrice').setValue(get_data.price);
            addGrid = comboGrid;
            Ext.getCmp('addWin').add(addGrid);
        }
    });
}

//保存
function comboSave(get_data) {
    //判斷頁面中是否存在相同的外場編號
    var Hasrepeat = false; //初始不存在
    for (var i = 0, j = productStore.getCount(); i < j; i++) {
        for (var m = (i + 1); m < j; m++) {
            if (productStore.getAt(i).data.channel_detail_id == productStore.getAt(m).data.channel_detail_id) {
                Hasrepeat = true;
                break;
            }
        }
    }
    if (Hasrepeat == true) {
        Ext.Msg.alert(INFORMATION, HAS_REPEAT_INFO);
        return false;
    }
    //和數據庫進行比較
    HasRepeartSql(get_data);
    if (Hasbl == false) {
        return false;
    }
    //-----end
    var saveData = "[";
    for (var i = 0, j = productStore.getCount(); i < j; i++) {
        var NewGroup_item_id = "";
        var pd = productStore.getAt(i).data;
        //ProductMapSet 數據
        var strChild = "[";
        for (var m = 1; m <= columnCount; m++) {
            if (strChild != "[" && eval("pd.product_num_" + m + "") != 0) {
                strChild += ",";
            }
            if (eval("pd.product_num_" + m + "") != 0) {
                strChild += "{item_id:" + eval("pd.item_id_" + m + "") + ",set_num:" + eval("pd.product_num_" + m + "") + "}";
            }
        }
        strChild += "]";


        //ProductItemMap 數據
        if (i > 0) {
            saveData += ",";
        }
        for (var k = 0, l = columnCount; k < l; k++) {
            for (var m = 1; m <= l; m++) {
                if ((pd.group_item_id.split(',')[k] == eval("pd.item_id_" + m + "")) && eval("pd.product_num_" + m + "") != 0) {
                    if (NewGroup_item_id != "") {
                        NewGroup_item_id += ",";
                    }
                    NewGroup_item_id += eval("pd.item_id_" + m + "");
                }
            }
        }
        saveData += "{rid:" + pd.rid + ",channel_id:" + Ext.getCmp("comboxOutSite").getValue() + ",price_master_id:" + Ext.getCmp("comboxSitePrice").getValue() + ",channel_detail_id:'" + pd.channel_detail_id + "'";;//edit by xiangwang0413w 2014/07/2 根根據站台價格查詢商品對照
        saveData += ",product_name:'" + pd.product_name + "',product_cost:" + pd.product_cost + ",product_price:" + pd.product_price + "";
        saveData += ",product_id:" + get_data.parent_id + ",group_item_id:'" + NewGroup_item_id + "',strChild:'" + strChild + "'}";
    }
    saveData += "]";
    Ext.Ajax.request({
        url: '/ProductItemMap/ComboCompareSave',
        params: {
            ChannelId: Ext.htmlEncode(Ext.getCmp("comboxOutSite").getValue()),
            dataJson: saveData
        },
        success: function (response) {
            var data = Ext.decode(response.responseText);
            if (data.success) {
                Ext.Msg.alert(INFORMATION, SUCCESS, function () {
                    productStore.load({
                        params: {
                            cId: Ext.htmlEncode(Ext.getCmp("comboxOutSite").getValue()),
                            ProductId: Ext.htmlEncode(get_data.parent_id),
                            pmId: Ext.htmlEncode(Ext.getCmp("comboxSitePrice").getValue())  //edit by xiangwang0413w 2014/07/4
                        },
                        callback: function () {
                            if (c_get_data.combination == 3 && c_get_data.buy_limit != 1) {
                                showExistData(productStore);
                            }
                        }
                    });
                });
            }
            else {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }

        }
    });
}


//刪除
function comboDelete(rowid) {
    Ext.Msg.confirm(INFORMATION, CONFIRM_DELETE, function (btn) {
        if (btn == 'yes') {
            var cdata = productStore.getAt(rowid).data;
            //臨時數據
            if (!cdata.rid) {
                productStore.removeAt(rowid);
            }
            else {
                //數據庫中數據 
                Ext.Ajax.request({
                    url: '/ProductItemMap/ComboMapDelele',
                    params: { rid: cdata.rid },
                    success: function (response) {
                        var resText = Ext.decode("(" + response.responseText + ")");
                        Ext.Msg.alert(INFORMATION, SUCCESS, function () {
                            productStore.load({
                                params: {
                                    cId: Ext.htmlEncode(Ext.getCmp("comboxOutSite").getValue()),
                                    ProductId: Ext.htmlEncode(Ext.getCmp("txtProductId").getValue()),
                                    pmId: Ext.htmlEncode(Ext.getCmp("comboxSitePrice").getValue())
                                },
                                callback: function () {
                                    if (c_get_data.buy_limit == 1) {
                                        for (var i = 0, j = productStore.getCount(); i < j; i++) {
                                            for (var m = 1; m <= columnCount; m++) {
                                                productStore.getAt(i).set('product_num_' + m, 1);
                                            }
                                        }
                                    }
                                    if (c_get_data.combination == 3 && c_get_data.buy_limit != 1) {
                                        showExistData(productStore);
                                    }
                                }
                            });
                        });
                    },
                    failure: function (response) {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });

            }


        }
    });

}



function orderAsc(data) {
    var arry = data.split(',');
    var temp;
    for (var i = 0, j = arry.length; i < j - 1; i++) {
        for (var m = 0, n = j - i - 1; m < n; m++) {
            if (arry[m] > arry[m + 1]) {
                temp = arry[m];
                arry[m] = arry[m + 1];
                arry[m + 1] = temp;
            }
        }
    }
    temp = '';
    for (var i = 0, j = arry.length; i < j; i++) {
        if (i > 0) {
            temp += ",";
        }
        temp += arry[i];
    }
    return temp;
}
//重數據庫驗證是否存在相同的賣場商品ID
function HasRepeartSql(get_data) {
    var saveData = "[";
    for (var i = 0, j = productStore.getCount(); i < j; i++) {
        var pd = productStore.getAt(i).data;
        //ProductItemMap 數據
        if (i > 0) {
            saveData += ",";
        }
        saveData += "{rid:" + pd.rid + ",channel_id:" + Ext.getCmp("comboxOutSite").getValue() + ",channel_detail_id:'" + pd.channel_detail_id + "',msg:" + productStore.getAt(i).isModified('channel_detail_id') + "}";
    }
    saveData += "]";

    Ext.Ajax.request({
        url: '/ProductItemMap/HasRepeartSql',
        method: 'POST',
        async: false,
        params: {
            ChannelId: Ext.htmlEncode(Ext.getCmp("comboxOutSite").getValue()),
            dataJson: saveData
        },
        success: function (response) {
            var data = Ext.decode(response.responseText); //返回的信息
            if (data.success) {
                Hasbl = true;
            }
            else {
                Ext.Msg.alert(INFORMATION, data.data);
                Hasbl = false;
            }
        }
    });
}
