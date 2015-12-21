var pageSize = 20;
var upload;
var uploadForm;
var myMask;
var addGrid;
var productStore;

FILE_PATH = "";
Ext.define('GIGADE.PRODUCTITEMMAP', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'rid', type: 'int' },
    { name: 'product_id', type: 'int' },
    { name: 'groupcombo_product_id', type: 'int' },
    { name: 'channel_name_full', type: 'string' },
    { name: 'group_item_id', type: 'string' },
    { name: 'item_id', type: 'int' },
    { name: 'product_name', type: 'string' },
    { name: 'channel_detail_id', type: 'string' },
    { name: 'product_cost', type: 'decimal' },
    { name: 'product_price', type: 'decimal' },
    { name: 'msg', type: 'string' },
    //edit by xiangwang0413w 2014/07/10 增加三個欄位，站台、會員等級、會員email
    { name: 'site_name', type: 'string' },
    { name: 'user_level_name', type: 'string' },
    { name: 'user_email', type: 'string' } 
    ]
});
var productMapStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.PRODUCTITEMMAP',
    pageSize: pageSize,
    autoDestroy: true,
    proxy: {
        type: 'ajax',
        url: '/ProductItemMap/QueryProductItemMap',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
//上傳時錯誤數據Store
var errorMapStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.PRODUCTITEMMAP',
    autoDestroy: false,
    groupField: 'msg',
    pageSize: pageSize,
    listeners: {
        datachanged: function (store) {
            if (store.getCount() > 0) {
                Ext.getCmp("btnContinueImport").setDisabled(false);
            }
        }
    }
});
var conditionArray = [
         { ParameterCode: '1', ParameterContent: '商品編號' },
         { ParameterCode: '2', ParameterContent: PRODUCT_ITEM_ID },
         { ParameterCode: '4', ParameterContent: '外站名稱' },
         { ParameterCode: '5', ParameterContent: '外站商品名稱' },
         { ParameterCode: '6', ParameterContent: '外站商品編號' }
];
var conditionStore = Ext.create('Ext.data.Store', {
    fields: ['ParameterCode', 'ParameterContent'],
    autoDestroy: false,
    data: conditionArray
});
var c_outsiteStore = Ext.create('Ext.data.Store', {
    fields: ['channel_id', 'channel_name_full'],
    proxy: {
        type: 'ajax',
        url: '/ProductItemMap/GetOutSite',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    },
    autoLoad: true
});
productMapStore.on("beforeload", function () {
    Ext.apply(productMapStore.proxy.extraParams,
    {
        condition: Ext.htmlEncode(Ext.getCmp('searConditionCombox').getValue()),
        value: Ext.htmlEncode(Ext.getCmp('searContent').getValue())
    });
});
//商品對照查詢
query = function () {
    var comb = Ext.getCmp('searConditionCombox');
    if (!comb.isValid()) {
        return;
    }
    productMapStore.removeAll();
    productMapStore.loadPage(1);
};
//錯誤數據顯示
var errorGrid = Ext.create('Ext.grid.Panel', {
    id: 'errorGrid',
    autoScroll: true,
    width: '100%',
    store: errorMapStore,
    height: document.documentElement.clientHeight - (document.documentElement.clientHeight / 6) - 200,
    columns: [{ header: NO, xtype: 'rownumberer', width: 80, align: 'center' },
                  { header: PRODCUT_ID, dataIndex: 'product_id', width: 100, align: 'center' },
                  { header: PRODUCT_ITEM_ID, xtype: 'templatecolumn', tpl: Ext.create('Ext.XTemplate', '{[values.item_id==0?values.group_item_id:values.item_id]}'), width: 80, align: 'center' },
    //{ header: OUTSITE_PRODUCT_NAME, dataIndex: 'product_name', width: 180, align: 'center' },
                  { header: OUTSITE_PRODUCT_ID, dataIndex: 'channel_detail_id', width: 100, align: 'center' },
    // { header: OUTSITE_PRODUCT_COST, dataIndex: 'product_cost', width: 100, align: 'center' },
    // { header: OUTSITE_PRODUCT_PRICE, dataIndex: 'product_price', width: 100, align: 'center' },
                  { header: ERROR_MSG, dataIndex: 'msg', width: 200, align: 'left' }
    ],
    listeners: {
        scrollershow: function (scroller) {
            if (scroller && scroller.scrollEl) {
                scroller.clearManagedListeners();
                scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
            }
        }
    }
});
//Excel文件驗證
Ext.apply(Ext.form.field.VTypes, {
    excelFormat: function (v) {
        var extension = v.substring(v.lastIndexOf("."));
        if (extension != ".xls" && extension != ".xlsx") {
            return false;
        }
        else {
            return true;
        }
    },
    excelFormatText: EXCEL_FORMAT_ERROR
});
/* AddorModifyCompare */
var outsiteStore = Ext.create('Ext.data.Store', {
    fields: ['channel_id', 'channel_name_full'],
    proxy: {
        type: 'ajax',
        url: '/ProductItemMap/GetOutSite',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    },
    autoLoad: true
});
//站台價格store
var sitePriceStore = Ext.create('Ext.data.Store', {
    fields: ['price_master_id', 'site_price_option'],
    proxy: {
        type: 'ajax',
        url: '/ProductItemMap/GetSitePriceOption',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    },
    autoLoad: false
});
//對照複製
function comboCopy(get_data) {
    for (var i = 0, j = productStore.getCount() ; i < j; i++) {
        var data = productStore.getAt(i);
        data.set('product_name', get_data.product_name);
        data.set('product_cost', get_data.cost);
        data.set('product_price', get_data.price);
    }
}
add_query = function (pId, pmId) {
    if (addGrid) {
        addGrid.destroy();
    }
    Ext.Ajax.request({
        url: '/ProductItemMap/QueryCombination',
        params: {
            ProductId: Ext.htmlEncode(pId),
            PriceMasterId: Ext.htmlEncode(pmId)
        },
        success: function (response) {
            if (response && response.responseText) {
                var data = Ext.JSON.decode(response.responseText);
                if (data && data.success) {
                    switch (data.data.combination) {
                        case 0:
                        case 1: showSingleAdd(data.data); break;        //單一商品對照新增
                        case 2:
                        case 3:
                        case 4: showComboAdd(data.data); break;         //組合商品對照新增
                        default: break;
                    }
                }
                else {
                    Ext.Msg.alert('提示', '商品編號不正確');
                }
            }
        }
    });
}

//商品編號驗證  edit by Jiajun 2014/10/10
Ext.apply(Ext.form.field.VTypes, {
    regxPNo: function (val, field) {
        return /^\d{5,}$/.test(val) 
    },
    regxPNoText: INPUT_NUMBER
});

function deleteProItemMap(rid, pno, cid) {
    if (rid == 0) {
        return;
    }
    Ext.Msg.confirm(INFORMATION, CONFIRM_DELETE, function (btn) {
        if (btn == 'yes') {
            Ext.Ajax.request({
                url: '/ProductItemMap/DeleteProductMap',
                params: { rid: rid, pno: pno, cid: cid },
                success: function (response) {
                    var resText = eval("(" + response.responseText + ")");
                    Ext.Msg.alert(INFORMATION, resText.rpackCode);
                    addGrid.getStore().load({
                        params: {
                            cId: Ext.htmlEncode(Ext.getCmp("comboxOutSite").getValue()),
                            pNo: Ext.htmlEncode(Ext.getCmp("txtProductId").getValue())
                        }
                    });
                },
                failure: function (response) {
                    var resText = eval("(" + response.responseText + ")");
                    Ext.Msg.alert(INFORMATION, resText.rpackCode);
                }
            });
        }
    });
}
addProCompare = function () {
    var addWin = Ext.create('Ext.window.Window', {
        id: 'addWin',
        title: ADD_MODIFY_COMPARE,
        minWidth: 600,
        width: document.documentElement.clientWidth - (document.documentElement.clientWidth / 6),
        height: document.documentElement.clientHeight - (document.documentElement.clientHeight / 3),
        closeAction: 'destroy',
        resizable: false,
        modal: true,
        constrain: true,
        plain: true,
        bodyStyle: 'background-color:white',
        layout: 'fit',
        tbar: [
        {
            xtype: 'combobox',
            editable: false,
            fieldLabel: OUTSITE_NAME,
            labelWidth: 65,
            store: outsiteStore,
            displayField: 'channel_name_full',
            valueField: 'channel_id',
            forceSelection: true,
            id: 'comboxOutSite',
            listeners: {
                change: function () {
                    if (Ext.getCmp('addGrid')) {
                        Ext.getCmp('addGrid').getStore().removeAll();
                    }
                }
            }
        }, 
        {
            xtype: 'textfield',
            fieldLabel: GIGADE_PRODUCT_ID,
            vtype: 'regxPNo',
            labelWidth: 100,
            msgTarget: 'side',
            id: 'txtProductId',
            enableKeyEvents: true,
            allowBlank: false,
            editable: false,
            listeners: {
                specialkey: function (field, e) {
                    if (e.getKey() == e.ENTER) {
                        if (field.value == null) {
                            return;
                        }
                        if (field.isValid()) { //edit by Jiajun 2014/10/10
                            //sitePriceStore.removeAll();
                            //if (Ext.getCmp('comboxSitePrice').combo)
                            //    delete Ext.getCmp('comboxSitePrice').combo.lastQuery;
                            var pId = this.value;
                            sitePriceStore.load({
                                params: {
                                    productId: pId,
                                    channelId: Ext.htmlEncode(Ext.getCmp("comboxOutSite").getValue())
                                },
                                callback: function (records, operation, success) {
                                    if (records.length) {
                                        Ext.getCmp("comboxSitePrice").setValue(records[0].data["price_master_id"]);
                                    }
                                    add_query(pId, Ext.getCmp("comboxSitePrice").getValue());
                                }
                            });//add by xiangwang0413w 2014/07/02 根据商品id查询站点价格下拉列表选项
                        }
                        else {
                            if (Ext.getCmp('addGrid')) {
                                Ext.getCmp('addGrid').getStore().removeAll();
                            }
                        }
                    }
                }
            }
        },
        {
            xtype: 'combobox',
            editable: false,
            fieldLabel: SITE_PRICE,
            labelWidth: 65,
            width: 310,
            store: sitePriceStore,
            queryMode: 'local',
            displayField: 'site_price_option',
            valueField: 'price_master_id',
            id: 'comboxSitePrice',
            listConfig: { loadMask: false },
            listeners: {
                select: function () {
                    add_query(Ext.getCmp('txtProductId').getValue(), Ext.getCmp("comboxSitePrice").getValue());
                }
            }
        }],
        listeners: {
            show: function () {
                if (Ext.getCmp('addGrid')) {
                    Ext.getCmp('addGrid').getStore().removeAll();
                }
                Ext.getCmp('comboxSitePrice').getStore().removeAll();
            },
            destroy: function () {
                productMapStore.load();
            }
        }

    });

    addWin.show(this);
}
RemitExc = function () {//20140805添加匯出Exc
    EditloadForm = Ext.create('Ext.form.Panel', {
        width: '100%',
        border: false,
        frame: true,
        waitMsgTarget: true,
        url: '/ProductItemMap/OutExcel',
        buttons: [{
            text: EXCEL,
            formBind: true,
            disabled: true,
            handler: function () {
                window.open('/ProductItemMap/OutExcel?rid=' + Ext.getCmp("rid").getValue() + '&channelid=' + Ext.getCmp("channel_id_outexcel").getValue());
            }
        }],
        items: [
        {
            xtype: 'combobox',
            fieldLabel: OUTSITE_NAME,
            id: 'channel_id_outexcel',
            name: 'channel_id_outexcel',
            displayField: 'channel_name_full',
            valueField: 'channel_id',
            store: c_outsiteStore,
            msgTarget: 'side',
            blankText: CHOOSE_OUTSITE,
            anchor: '90%'
        },
        {
            fieldLabel: STRATID,
            xtype: 'numberfield',
            id: 'rid',
            name: 'rid',
            anchor: '90%'
        }]
    });
    Editload = Ext.create('widget.window', {
        title: RESOURCE_IMPORT,
        closable: true,
        modal: true,
        constrain: true,
        width: 500,
        layout: 'anchor',
        bodyStyle: 'padding: 5px;',
        items: [EditloadForm],
        resizable: false
    });
    Editload.show(this);
}
Ext.onReady(function () {
    var productGrid = Ext.create('Ext.grid.Panel', {
        id: 'productGrid',
        store: productMapStore,
        width: document.documentElement.clientWidth,
        //scroll: false,
        columns: [
                  //{ header: NO, xtype: 'rownumberer', width: 100, align: 'center' },
                  //add by hjiajun1211w 2014/08/04 修改顯示的rid，保持跟數據庫一致
                  { header: NO, dataIndex: 'rid', width: 180, align: 'center' },
                  { header: OUTSITE_NAME, dataIndex: 'channel_name_full', width: 180, align: 'center' },
                  { header: OUTSITE_PRODUCT_NAME, dataIndex: 'product_name', width: 280, align: 'left' },
                  { header: OUTSITE_PRODUCT_ID, dataIndex: 'channel_detail_id', width: 150, align: 'center' },
                  { header: OUTSITE_PRODUCT_PRICE, dataIndex: 'product_price', width: 150, align: 'center' },
                  { header: PRODCUT_ID, xtype: 'templatecolumn', tpl: Ext.create('Ext.XTemplate', '{[values.product_id==0?values.groupcombo_product_id:values.product_id]}'), width: 100, align: 'center' },
                  { header: PRODUCT_ITEM_ID, xtype: 'templatecolumn', tpl: Ext.create('Ext.XTemplate', '{[values.item_id==0?values.group_item_id:values.item_id]}'), width: 100, align: 'center' },
                  { header: OUTSITE_PRODUCT_COST, dataIndex: 'product_cost', hidden: true, width: 150, align: 'center' },
                  //edit by xiangwang0413w 2014/07/10 增加三個欄位，站台、會員等級、會員email
                  { header: SITE_NAME, dataIndex: 'site_name', width: 150, align: 'center' },
                  { header: USER_LEVEL_NAME, dataIndex: 'user_level_name', width: 150, align: 'center' },
                  { header: USER_EAMIL, dataIndex: 'user_email', width: 150, align: 'center' }

        ],
        tbar: [
        {
            xtype: 'combobox',
            editable: false,
            fieldLabel: SEARCH_CONDITION,
            labelWidth: 60,
            store: conditionStore,
            allowBlank: false,
            blankText: CHOOSE_CONDITION,
            displayField: 'ParameterContent',
            valueField: 'ParameterCode',
            forceSelection: true,
            id: 'searConditionCombox'
        }, 
        {
            xtype: 'textfield',
            fieldLabel: SEARCH_CONTENT,
            labelWidth: 60,
            id: 'searContent',
            enableKeyEvents: true,
            listeners: {
                keydown: function (field, e) {
                    if (e.keyCode == 13) {
                        query();
                    }

                }
            }
        }, 
        {
            xtype: 'button',
            text: SEARCH,
            border: 2,
            iconCls: 'ui-icon ui-icon-search-2 ',
            handler: query
        }],
        dockedItems: [{
            xtype: 'toolbar',
            defaultType: 'button',
            items: [
            {
                iconCls: 'ui-icon ui-icon-add',
                id: 'btnAddCompare',
                hidden: true,
                text: ADD_MODIFY_COMPARE,
                handler: addProCompare
            }, 
            {
                iconCls: 'ui-icon ui-icon-paper-up',
                id: 'btnImportCompare',
                hidden: true,
                text: IMPORT_COMPARE,
                handler: function () {
//                    if (upload) {
//                        Ext.getCmp("uploadFile").reset();
//                        Ext.getCmp("w_comboxOutSite").reset();
//                        Ext.getCmp("btnContinueImport").setDisabled(true);
//                        errorMapStore.removeAll();
//                        upload.show(this);
//                        return;
//                    }              //edit by wangwei0216w 2014/8/12 屏蔽掉以上代碼 以 解決打開 <匯出賣場商品> 窗口 或者 <賣場商品對照批次修改> 窗口 后 無法打開<匯入商品對照>窗口BUG
                    uploadForm = Ext.create('Ext.form.Panel', {
                        width: '100%',
                        height: 130,
                        border: false,
                        frame: true,
                        waitMsgTarget: true,
                        items: [
                        {
                            xtype: 'combobox',
                            editable: false,
                            fieldLabel: OUTSITE_NAME,
                            labelWidth: 60,
                            store: c_outsiteStore,
                            displayField: 'channel_name_full',
                            valueField: 'channel_id',
                            forceSelection: true,
                            allowBlank: false,
                            msgTarget: 'side',
                            blankText: CHOOSE_OUTSITE,
                            id: 'w_comboxOutSite',
                            name: 'w_comboxOutSite'
                        }, 
                        {
                            xtype: 'filefield',
                            id: 'uploadFile',
                            fieldLabel: CHOOSE_FILE,
                            width: 400,
                            labelWidth: 60,
                            msgTarget: 'side',
                            allowBlank: false,
                            blankText: CHOOSE_FILE,
                            vtype: 'excelFormat',
                            anchor: '80%',
                            buttonText: BROWSER
                        }, 
                        {
                            xtype: 'button',
                            text: IMPORT,
                            width: 100,
                            disabled: true,
                            formBind: true,
                            handler: function () {
                                var uploadFile = Ext.getCmp("uploadFile");
                                var combox = Ext.getCmp("w_comboxOutSite");
                                errorMapStore.removeAll();
                                if (combox.isValid() && uploadFile.isValid()) {
                                    uploadForm.submit({
                                        url: '/ProductItemMap/DoUpload',
                                        waitMsg: IMPORT_NOW,
                                        success: function (form, action) {
                                            if (action.result.success) {
                                                if (action.result.headerError) {
                                                    Ext.Msg.alert(NOTICE, EXCEL_HEADER_ERROR);
                                                    return;
                                                }
                                                if (action.result.error) {
                                                    if (action.result.data.length > 0) {
                                                        errorMapStore.removeAll();
                                                        errorMapStore.loadData(action.result.data);
                                                        FILE_PATH = action.result.path;
                                                    }
                                                }
                                                else {
                                                    Ext.Msg.alert(NOTICE, action.result.msg);
                                                    upload.hide();
                                                }
                                            }
                                        },
                                        failure: function (form, action) {
                                            Ext.Msg.alert(NOTICE, action.result.msg);
                                        }
                                    });
                                }
                            }
                        }, 
                        {
                            xtype: 'panel',
                            plain: true,
                            frame: true,
                            bodyStyle: 'padding:5px 0px 5px 0px; text-align:center',
                            border: 0,
                            html: '<a style=" text-decoration:none;color:Blue;" href="/Ashx/ProductItemMapExcel.ashx">' + TEMPLATE_DOWN + '</a>'
                        }]
                    });
                    upload = Ext.create('widget.window', {
                        title: RESOURCE_IMPORT,
                        closable: true,
                        modal: true,
                        autoScroll: true,
                        constrain: true,
                        minWidth: 400,
                        //closeAction: 'hide',
                        width: document.documentElement.clientWidth - (document.documentElement.clientWidth / 1.5),
                        height: document.documentElement.clientHeight - (document.documentElement.clientHeight / 6),
                        layout: 'anchor',
                        bodyStyle: 'padding: 5px;',
                        items: [uploadForm, errorGrid],
                        resizable: false,
                        bbar: [{
                            type: 'button',
                            id: 'btnContinueImport',
                            disabled: true,
                            text: CONTINUE_IMPORT,
                            handler: function () {
                                var date = new Date();
                                if (!myMask) {
                                    myMask = new Ext.LoadMask(Ext.getCmp("errorGrid"), { msg: LOADING });
                                }
                                myMask.show();
                                Ext.Ajax.request({
                                    url: '/ProductItemMap/ContinueUpload?t=' + date.getMilliseconds(),
                                    waitMsgTarget: true,
                                    method: 'POST',
                                    params: {
                                        'fname': FILE_PATH,
                                        'outSite': Ext.getCmp("w_comboxOutSite").getValue()
                                    },
                                    success: function (response, opts) {
                                        myMask.hide();
                                        var resText = eval("(" + response.responseText + ")");
                                        uploadForm.getForm().reset();
                                        errorMapStore.removeAll();
                                        Ext.getCmp("btnContinueImport").setDisabled(true);
                                        Ext.Msg.alert(NOTICE, resText.msg);
                                    },
                                    failure: function (response) {
                                        var resText = eval("(" + response.responseText + ")");
                                    }
                                });
                            }
                        }]
                    });
                    upload.show();
                }
            }, 
            {//新增20140805
                iconCls: 'ui-icon ui-icon-paper-down',
                id: 'btnAddExc',
                hidden: true,
                text: PRODUCTEXCEL,//2
                handler: RemitExc
            },
            {
                iconCls: 'ui-icon ui-icon-data-change',
                id: 'btn_Edit',
                hidden: true,
                text: PRODUCTEDIT, //3
                handler: function () {
                upEditloadForm = Ext.create('Ext.form.Panel', {
                    border: 0,
                    frame: true,
                    waitMsgTarget: true,
                    items: [
                    {
                        xtype: 'combobox',
                        id: 'channel',
                        name: 'channel',
                        padding: '10 0 5 0',
                        anchor: '80%',
                        valueField: 'channel_id',
                        displayField: 'channel_name_full',
                        store: c_outsiteStore,
                        fieldLabel: OUTSITE_NAME,
                        forceSelection: true,
                        allowBlank: false,
                        msgTarget: 'side',
                        blankText: CHOOSE_OUTSITE
                    }, 
                    {
                        xtype: 'filefield',
                        id: 'UPfile',
                        fieldLabel: CHOOSE_FILE,
                        padding: '10 0 5 0',
                        anchor: '80%',
                        msgTarget: 'side',
                        allowBlank: false,
                        blankText: CHOOSE_FILE,
                        vtype: 'excelFormat',
                        buttonText: BROWSER
                    }, 
                    {
                        xtype: 'button',
                        text: IMPORT,
                        width: 100,
                        disabled: true,
                        formBind: true,
                        handler: function () {
                            var uploadFile = Ext.getCmp("UPfile");
                            var combox = Ext.getCmp("channel");
                            if (combox.isValid() && uploadFile.isValid()) {
                                upEditloadForm.submit({  // edit by zhuoqin0830w  2015/12/17   原因：頁面顯示的from與修改之前所調用的from不一致，所以導致點擊匯入按鈕時沒有任何反應
                                    url: '/ProductItemMap/EditUpload',
                                    waitMsg: IMPORT_NOW,
                                    success: function (form, action) {
                                        if (action.result.success) {
                                            Ext.Msg.alert(NOTICE, action.result.msg);
                                            return;
                                        }
                                    },
                                    failure: function (form, action) {
                                        Ext.Msg.alert(NOTICE, action.result.msg);
                                    }
                                });
                            }
                        }
                    }]
                });
                upEditload = Ext.create('widget.window', {
                    title: RESOURCE_IMPORT,
                    closable: true,
                    modal: true,
                    constrain: true,
                    width: 400,
                    layout: 'anchor',
                    bodyStyle: 'padding: 5px;',
                    items: [upEditloadForm],
                    resizable: false
                });
                upEditload.show(this);
                }
            }, '->', { text: ' ' }]            
        }],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: productMapStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        })
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [productGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                productGrid.width = document.documentElement.clientWidth;
                if (Ext.getCmp('addWin')) {
                    Ext.getCmp('addWin').setWidth(document.documentElement.clientWidth - (document.documentElement.clientWidth / 6));
                    Ext.getCmp('addWin').setHeight(document.documentElement.clientHeight - (document.documentElement.clientHeight / 3));
                }
                this.doLayout();
            }
        }
    });
    ToolAuthority();

    Ext.getCmp("productGrid").store.load();

});
