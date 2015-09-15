var pageSize = 25;
//群組管理Model
Ext.define('gigade.CateBrandModel', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "row_id", type: "int" },
    { name: "banner_cate_id", type: "int" },
    { name: "banner_catename", type: "string" },
    { name: "category_id", type: "int" },
    { name: "category_name", type: "string" },
    { name: "category_father_id", type: "int" },
    { name: "category_father_name", type: "string" },
    { name: "depth", type: "int" },
    { name: "brand_id", type: "int" },
    { name: "brand_name", type: "string" },
    { name: "createdate", type: "string" }
    ]
});
var CateBrandStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    autoLoad: false,
    model: 'gigade.CateBrandModel',
    proxy: {
        type: 'ajax',
        url: '/ProductCategory/GetCateBrandList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
CateBrandStore.on('beforeload', function () {
    Ext.apply(CateBrandStore.proxy.extraParams,
    {
        searchCate: Ext.getCmp('searchCate').getValue(),
        searchBrand: Ext.getCmp('searchBrand').getValue(),
        banner_id: Ext.getCmp('banner_id').getValue()
    });
});
//banner_cate類別
Ext.define("gigade.bannerCate", {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'parameterCode', type: 'string' },
    { name: 'parameterName', type: 'string' }]
});
var bannerCateStore = Ext.create("Ext.data.Store", {
    model: 'gigade.bannerCate',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=banner_cate',
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
    var searchForm = Ext.create('Ext.form.Panel', {
        id: 'searchForm',
        layout: 'anchor',
        height: 50,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [{
            xtype: 'fieldcontainer',
            combineErrors: true,
            layout: 'hbox',
            items: [
            {
                xtype: 'textfield',
                id: 'searchCate',
                margin: "0 5 0 0",
                fieldLabel: '類別編號/名稱',
                labelWidth: 90,
                listeners: {
                    specialkey: function (field, e) {
                        if (e.getKey() == e.ENTER) {
                            Search(1);
                        }
                    }
                }
            },
            {
                xtype: 'textfield',
                id: 'searchBrand',
                margin: "0 5 0 0",
                name: 'searchBrand',
                fieldLabel: '品牌編號/名稱',
                labelWidth: 90,
                listeners: {
                    specialkey: function (field, e) {
                        if (e.getKey() == e.ENTER) {
                            Search(1);
                        }
                    }
                }
            },
            {
                xtype: 'combobox',
                allowBlank: true,
                fieldLabel: BANNERCATID,
                editable: false,
                id: 'banner_id',
                labelWidth: 60,
                width: 180,
                margin: '0 30 0 0',
                name: 'banner_id',
                hiddenName: 'category_id',
                colName: 'category_name',
                store: bannerCateStore,
                displayField: 'parameterName',
                valueField: 'parameterCode',
                typeAhead: true,
                forceSelection: false,
                emptyText: SELECT
            },
            {
                xtype: 'button',
                margin: '0 5 0 5',
                iconCls: 'ui-icon ui-icon-search-2',
                text: "查詢",
                handler: Search
            },
            {
                xtype: 'button',
                text: '重置',
                id: 'btn_reset',
                iconCls: 'ui-icon ui-icon-reset',
                listeners: {
                    click: function () {
                        Ext.getCmp('searchCate').setValue('');
                        Ext.getCmp('searchBrand').setValue('');
                        Ext.getCmp('banner_id').setValue("");
                    }
                }
            }
            ]
        }
        ]

    });

    var cateBrandGd = Ext.create('Ext.grid.Panel', {
        id: 'cateBrandGd',
        store: CateBrandStore,
        columnLines: true,
        width: document.documentElement.clientWidth,
        height: document.documentElement.clientheight - 50,
        frame: true,
        flex: 7,
        viewConfig: {
            enableTextSelection: true,
            stripeRows: false,
            getRowClass: function (record, rowIndex, rowParams, store) {
                return "x-selectable";
            }
        },
        columns: [
        { header: "編號", dataIndex: 'row_id', flex: 0.9, align: 'center', align: 'center' },
        { header: "專區類別編號", dataIndex: 'banner_cate_id', flex: 0.9, align: 'center', align: 'center' },
        { header: "專區類別名稱", dataIndex: 'banner_catename', flex: 2, align: 'center', align: 'center' },
        { header: CATEGORYID, dataIndex: 'category_id', flex: 0.9, align: 'center', align: 'center' },
        { header: CATEGORYNAME, dataIndex: 'category_name', flex: 2, align: 'center' },
          { header: "父類別編號", dataIndex: 'category_father_id', flex: 0.9, align: 'center', align: 'center' },
    { header: "父類別名稱", dataIndex: 'category_father_name', flex: 2, align: 'center' },
        { header: "深度", dataIndex: 'depth', flex: 0.9, align: 'center', align: 'center' },
        { header: "品牌編號", dataIndex: 'brand_id', flex: 0.9, align: 'center', align: 'center' },
        {
            header: "品牌名稱", dataIndex: 'brand_name', flex: 2, align: 'center'
        },
        {
            header: "創建時間", dataIndex: 'createdate', id: 'createdate', flex: 1.2, align: 'center'
        }],
        tbar: [
        { xtype: 'button', id: 'add', text: "附加專區類別品牌", iconCls: 'icon-add', disabled: false, handler: function () { onAddClick(); } }],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: CateBrandStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [searchForm, cateBrandGd],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                cateBrandGd.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
});

function Search() {
    if (Ext.getCmp("searchBrand").getValue() != "" || Ext.getCmp("searchCate").getValue() != "" || (Ext.getCmp("banner_id").getValue() != "" && Ext.getCmp("banner_id").getValue() != null)) {
        Ext.getCmp("cateBrandGd").store.loadPage(1, { params: { start: 0, limit: 25 } });
    }
    else {
        Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
    }
}
function onAddClick() {
    editFunction(null, CateBrandStore);
}
////添加
//onAddClick = function () {
//    Ext.MessageBox.show({
//        msg: 'Updating....',
//        wait: true
//    });
//    Ext.Ajax.request({
//        url: '/ProductCategory/SaveCateBrand',
//        success: function (form, action) {
//            var result = Ext.decode(form.responseText);
//            if (result.success == true) {
//                CateBrandStore.loadPage(1, { params: { start: 0, limit: 25 } });
//                Ext.Msg.alert(INFORMATION, SUCCESS);
//            }
//            else {
//                Ext.Msg.alert(INFORMATION, FAILURE);
//            }
//        }
//    })
//}