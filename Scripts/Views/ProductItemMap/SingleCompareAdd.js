

var singleGrid;
var singleCelledit;
var re = /}{/g;
var Hasbl = false; //存在為false

Ext.define('GIGADE.PRODUCTITEM', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'rid', type: 'int' },
    { name: 'product_id', type: 'int' },
    { name: 'channel_id', type: 'int' },
    { name: 'item_id', type: 'int' },
    { name: 'spec_id_1', type: 'int' },
    { name: 'spec_id_2', type: 'int' },
    { name: 'spec_name_1', type: 'string' },
    { name: 'spec_name_2', type: 'string' },
    { name: 'product_p_name', type: 'string' },
    { name: 'product_name', type: 'string' },
    { name: 'channel_detail_id', type: 'string' },
    { name: 'product_cost', type: 'int' },
    { name: 'product_price', type: 'int' },
    { name: 'cost', type: 'int' },
    { name: 'price', type: 'int' }
    ]
});


function showSingleAdd(data) {

    productStore = Ext.create('Ext.data.Store', {
        model: 'GIGADE.PRODUCTITEM',
        pageSize: pageSize,
        proxy: {
            type: 'ajax',
            url: '/ProductItemMap/QueryProductByNo',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data',
                totalProperty: 'totalCount'
            }
        }
    });

    productStore.on('beforeload', function () {
        Ext.apply(productStore.proxy.extraParams,
             {
                 cId: Ext.htmlEncode(Ext.getCmp("comboxOutSite").getValue()),
                 pNo: Ext.htmlEncode(Ext.getCmp("txtProductId").getValue()),
                 pmId: Ext.htmlEncode(Ext.getCmp("comboxSitePrice").getValue())//edit by xiangwang0413w 2014/07/20 根根據站台價格查詢商品對照
             });
    });


    var sm = Ext.create('Ext.selection.CheckboxModel');
    singleCelledit = Ext.create('Ext.grid.plugin.CellEditing', {
        clicksToEdit: 1
    });

    singleGrid = Ext.create('Ext.grid.Panel', {
        id: 'singleGrid',
        // height: document.documentElement.clientHeight - (document.documentElement.clientHeight / 2),
        autoScroll: true,
        frame: true,
        store: productStore,
        columns: [
                      { header: NO, xtype: 'rownumberer', width: 50 },
                      //edit by xiangwang0413w 2014/07/11 調整grid結構，把可編輯列顯示在前面
                      { header: OUTSITE_PRODUCT_NAME, dataIndex: 'product_name', width: 260, editor: { xtype: 'textfield', allowBlank: false } },
                      { header: OUTSITE_PRODUCT_ID, dataIndex: 'channel_detail_id', width: 150, editor: { xtype: 'textfield', allowBlank: false } },
                      { header: OUTSITE_PRODUCT_COST, dataIndex: 'product_cost', hidden: true, width: 120, editor: { xtype: 'numberfield', decimalPrecision: 0, minValue: 0, allowBlank: false } },
                      { header: OUTSITE_PRODUCT_PRICE, dataIndex: 'product_price', width: 120, editor: { xtype: 'numberfield', decimalPrecision: 0, minValue: 0, allowBlank: false } },
                      { header: GIGADE_SPEC_1, dataIndex: 'spec_name_1', width: 150 },
                      { header: GIGADE_SPEC_2, dataIndex: 'spec_name_2', width: 150 },
                      { header: BTN_DELETE, width: 50, xtype: 'templatecolumn', tpl: '<img alt=' + BTN_DELETE + ' src=\'../../../Content/img/icons/delete.gif\' onclick=\"deleteProItemMap(\'{rid}\',\'{channel_detail_id}\',\'{channel_id}\')\" />'}],
        plugins: [singleCelledit],
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
                width: 300,
                margin: '0 15 0 0',
                id: 'txtpName',
                fieldLabel: PRODUCT_NAME
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
                    var get_data = new Array();
                    var data = productStore.getAt(0).data;
                    get_data.push({ product_name: '' + data.product_p_name + '', cost: data.cost, price: data.price });
                    comboCopy(get_data[0]);
                }
            }]
        }],
        buttons: [{
            text: SAVE,
            handler: function () {
                var bool = false;
                for (var i = 0, j = productStore.getCount(); i < j; i++) {
                    var cdata = productStore.getAt(i).data;
                    if (cdata.channel_detail_id != '' && cdata.product_name != '' && cdata.product_price < cdata.price) {
                        bool = true;
                        break;
                    }

                }
                if (bool) {
                    Ext.Msg.confirm(INFORMATION, CHANNEL_PRICE_LOWER, function (btn) {
                        if (btn == "yes") {
                            singleSave();
                        }
                    });
                }
                else {
                    singleSave();
                }

            }
        }]
    });

    addGrid = singleGrid;
    Ext.getCmp('addWin').add(addGrid);

    productStore.removeAll();
    addGrid.store.loadPage(1, {
        params: {
            cId: Ext.htmlEncode(Ext.getCmp("comboxOutSite").getValue()),
            pNo: Ext.htmlEncode(data.parent_id)
        },
        callback: function () {
            if (Ext.getCmp("addWin")) {
                if (productStore.getCount() > 0) {
                    var data = productStore.getAt(0).data;
                    Ext.getCmp("txtpNo").setValue(data.product_id);
                    Ext.getCmp("txtpName").setValue(data.product_p_name);
                    Ext.getCmp("txtCost").setValue(data.cost);
                    Ext.getCmp("txtPrice").setValue(data.price);
                }
                addGrid.show();
            }
        }
    });



}



function singleSave() {
    singleCelledit.completeEdit();
    //獲取外站
    var channel_id = Ext.getCmp("comboxOutSite").getValue();
    var price_master_id = Ext.getCmp("comboxSitePrice").getValue();
    //判斷站台價格選項是否有選 edit by xiangwang
    if (!price_master_id) {
        Ext.Msg.alert(INFORMATION, NULL_SITE_PRICE);
        return false;
    }
    //判斷信息是否填寫完整
    for (var i = 0, j = productStore.getCount(); i < j; i++) {
        var data = productStore.getAt(i).data;
        if (data.product_name == "" || data.channel_detail_id == "") {
            Ext.Msg.alert(INFORMATION, COMPLETE_MAP_INFO);
            return false;
        }
    }
    //-----end
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
    //-----end

    //和數據庫進行比較
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
    HasRepeartMySql(saveData);
    if (Hasbl == false) {
        return false;
    }
    //-----end

    if (productStore.getCount() > 0) {
        var jsonSave = '[';
        for (var i = 0, j = productStore.getCount(); i < j; i++) {
            var cdata = productStore.getAt(i).data;
            if (cdata.channel_detail_id != '' && cdata.product_name != '') {
                jsonSave += Ext.String.format('{item_id:{0},channel_detail_id:\'{1}\',product_name:\'{2}\',product_cost:{3},product_price:{4},channel_id:{5},rid:{6},price_master_id:{7},product_id:{8}}', cdata.item_id, cdata.channel_detail_id, cdata.product_name, cdata.product_cost, cdata.product_price, Ext.htmlEncode(Ext.getCmp("comboxOutSite").getValue()), cdata.rid, Ext.htmlEncode(Ext.getCmp("comboxSitePrice").getValue()), cdata.product_id);
            }
        }
        jsonSave += ']';
        jsonSave = jsonSave.replace(re, "},{");
        Ext.Ajax.request({
            url: '/ProductItemMap/SaveProductItem',
            params: {
                jsonSave: jsonSave
            },
            success: function (response) {
                var res = Ext.decode(response.responseText);
                if (res.success) {
                    Ext.Msg.alert(INFORMATION, SUCCESS, function () {
                        addGrid.getStore().load();
                    });

                }

            }
        });
    }
}
//重數據庫驗證是否存在相同的賣場商品ID
function HasRepeartMySql(saveData) {

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
