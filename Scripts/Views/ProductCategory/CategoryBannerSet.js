Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
'Ext.form.Panel',
'Ext.ux.form.MultiSelect',
'Ext.ux.form.ItemSelector'
]);

//群組管理Model
Ext.define('gigade.ProductCategoryBannerModel', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "row_id", type: "int" },
    { name: "banner_cateid", type: "int" },
    { name: "banner_catename", type: "string" },
    { name: "category_id", type: "int" },
    { name: "category_name", type: "string" },
    { name: "category_father_id", type: "int" },
    { name: "category_father_name", type: "string" },
    { name: "category_sort", type: "int" },
    { name: "category_display", type: "int" },
    { name: "category_link_mode", type: "int" },
    { name: "createdate", type: "string" },
    { name: "createdate", type: "string" },
    { name: "create_ipfrom", type: "string" },
    { name: "status", type: "int" }
    ]
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
//重寫ext中的load方法，解決treestore加載時多次重複調用方法
Ext.override(Ext.data.TreeStore, {
    load: function (options) {
        options = options || {};
        options.params = options.params || {};
        var me = this,
        node = options.node || me.tree.getRootNode(),
        root;
        // If there is not a node it means the user hasnt defined a rootnode yet. In this case lets just
        // create one for them.
        if (!node) {
            node = me.setRootNode({
                expanded: true
            });
        }
        if (me.clearOnLoad) {
            node.removeAll(false);
        }
        Ext.applyIf(options, {
            node: node
        });
        options.params[me.nodeParam] = node ? node.getId() : 'root';
        if (node) {
            node.set('loading', true);
        }
        return me.callParent([options]);
    }
});
Ext.onReady(function () {
    //var leftW = 250; //左側樹狀結構的寬度
    //var theight = 320; //窗口的高度
    var pageSize = 25;
    ////獲取左邊的category樹結構(商品分類store)
    //var treeStore = Ext.create('Ext.data.TreeStore', {
    //    proxy: {
    //        type: 'ajax',
    //        url: '/ProductCategory/GetProductCategoryBanner',
    //        noCache: false,
    //        getMethod: function () { return 'get'; },
    //        actionMethods: 'post'
    //    },
    //    rootVisible: false,
    //    root: {
    //        text: BANNERCATID,
    //        expanded: true,
    //        children: []
    //    }
    //});
    //treeStore.load();
    //var treePanel = new Ext.tree.TreePanel({
    //    id: 'treePanel',
    //    region: 'west',
    //    width: leftW,
    //    border: 0,
    //    height: theight,
    //    store: treeStore
    //});

    var bannercateGd_store = Ext.create('Ext.data.Store', {
        pageSize: pageSize,
        model: 'gigade.ProductCategoryBannerModel',
        proxy: {
            type: 'ajax',
            url: '/ProductCategory/GetProCateBanList',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data',
                totalProperty: 'totalCount'
            }
        }
    });
    function SearchActivy() {     
        if (Ext.getCmp("banner_id").getValue() == "" || Ext.getCmp("banner_id").getValue() == null) {
            Ext.getCmp("bannercateGrid").store.removeAll();
            Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
        } else {
            bannercateGd_store.load();
        }

    }
    bannercateGd_store.on("beforeload", function () {
        Ext.apply(bannercateGd_store.proxy.extraParams, {
            banner_id: Ext.getCmp('banner_id').getValue()
        });
    });

    var sm = Ext.create('Ext.selection.CheckboxModel', {
        listeners: {
            selectionchange: function (sm, selections) {
                Ext.getCmp("bannercateGrid").down('#delete').setDisabled(selections.length == 0);
            }
        }
    });

    //顯示類別中商品的grid
    var bannercateGrid = new Ext.grid.Panel({
        id: 'bannercateGrid',
        store: bannercateGd_store,
        //region: 'center',
        autoScroll: true,
        border: 0,
        width: document.documentElement.clientWidth,
        height: document.documentElement.clientHeight,
        columns: [
        { header: BANNERCATID, dataIndex: 'banner_cateid', width: 120, align: 'center', hidden: true },
        { header: BANNERCATID, dataIndex: 'banner_catename', width: 120, align: 'center' },
        { header: CATEGORYID, dataIndex: 'category_id', width: 100, align: 'center' },
        { header: CATEGORYNAME, dataIndex: 'category_name', width: 240, align: 'center' },
        { header: FATHERCATEID, dataIndex: 'category_father_id', width: 100, align: 'center' },
        { header: FATHERCATENAME, dataIndex: 'category_father_name', width: 240, align: 'center' },
        { header: CATESOET, dataIndex: 'category_sort', width: 100, align: 'center' },
        {
            header: ISSHOW, dataIndex: 'category_display', width: 100, align: 'center',
            renderer: function (val) {
                if (val == 1) {
                    return SHOWSTATUS;
                }
                else {
                    return "<span style=' color:red'>" + HIDESTATUS + "</span>";
                }
            }
        },
        {
            header: LINKMODE, dataIndex: 'category_link_mode', width: 100, align: 'center',
            renderer: function (val) {
                if (val == 2) {
                    return NEWWIN;
                }
                else if (val == 1) {
                    return OLDWIN;
                }
            }
        },
        {
            header: BANNERCREATE, dataIndex: 'createdate', width: 100, align: 'center', hidden: true
        },
        {
            header: BANNERUPDATE, dataIndex: 'updatedate', width: 100, align: 'center', hidden: true
        }
        //,
        //{
        //    header: STATUS,
        //    dataIndex: 'status',
        //    width: 100,
        //    hidden: false,
        //    align: 'center',
        //    id: 'status',
        //    hidden: false,
        //    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
        //        if (value == 1) {
        //            return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.row_id + ")'><img hidValue='0' id='img" + record.data.row_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
        //        } else {
        //            return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.row_id + ")'><img hidValue='1' id='img" + record.data.row_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
        //        }
        //    }
        //}
        ],
        tbar: [
        { xtype: 'button', id: 'add', text: ADDCATEGORY, iconCls: 'icon-add', disabled: false, handler: function () { onAddClick(); } },
        {
            xtype: 'button', id: 'delete', text: DELETE, iconCls: 'icon-user-remove', hidden: true, disabled: true, handler: function () { onDeleteClick(); }

        },
        '->',
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
          }, {
              xtype: 'button',
              text: SEARCH,
              id: 'btn_search',
              handler: SearchActivy,
              iconCls: 'ui-icon ui-icon-search-2'
          }, {
              xtype: 'button',
              text: RESET,
              id: 'btn_reset',
              iconCls: 'ui-icon ui-icon-reset',
              listeners: {
                  click: function () {
                      Ext.getCmp('banner_id').setValue("");
                  }
              }
          }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: bannercateGd_store,
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
        },
        selModel: sm
    });

    ////右半部的下半部的grid
    //var centerSouthPan = new Ext.tab.Panel({
    //    region: 'center',
    //    items: [
    //    {
    //        title: BANNERCATID,
    //        items: [bannercateGrid]
    //    }
    //    ]
    //});

    ////整個右半部的panel
    //var centerPanel = new Ext.Panel({
    //    region: 'center',
    //    width: document.documentElement.clientWidth - leftW,
    //    height: document.documentElement.clientHeight - 22,
    //    autoScroll: true,
    //    items: [centerSouthPan]
    //}); //頁面佈局


    //Ext.create('Ext.container.Viewport', {
    //    layout: 'border',
    //    items: [centerPanel, treePanel]
    //});
    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [bannercateGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                bannercateGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    // bannercateGd_store.load({ params: { start: 0, limit: pageSize } });
    //添加
    function onAddClick() {
        editFunction(null, bannercateGd_store);
    }
    function onDeleteClick() {
        var row = Ext.getCmp('bannercateGrid').getSelectionModel().getSelection();
        if (row.length == 0) {
            Ext.Msg.alert(INFORMATION, NO_DATA);
        }
        else {
            Ext.Msg.confirm(INFORMATION, Ext.String.format(DELETE_INFO, row.length), function (btn) {
                if (btn == 'yes') {
                    var rowIDs = "";
                    for (var i = 0; i < row.length; i++) {
                        rowIDs += row[i].data.row_id + '|';
                    }
                    Ext.Ajax.request({
                        url: '/ProductCategory/DeleteProCateBan',
                        method: 'post',
                        params: { rowID: rowIDs },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);

                                bannercateGd_store.load();

                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        }
                    });
                }
            });
        }
    }
    //更改活動狀態(設置活動可用與不可用)
    UpdateActive = function (id, fatherId) {
        var activeValue = $("#img" + id).attr("hidValue");
        $.ajax({
            url: "/ProductCategory/UpdateState",
            data: {
                "id": id,
                "active": activeValue
            },
            type: "POST",
            dataType: "json",
            success: function (msg) {
                if (activeValue == 1) {
                    $("#img" + id).attr("hidValue", 0);
                    $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                    bannercateGd_store.load();
                } else {
                    $("#img" + id).attr("hidValue", 1);
                    $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                    bannercateGd_store.load();
                }
            },
            error: function (msg) {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        });
    };


});
