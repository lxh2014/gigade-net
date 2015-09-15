editFunction = function (row, store) {

    var bannerCateFatherStore = Ext.create("Ext.data.Store", {
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
    //群組管理Model
    Ext.define('gigade.cateBrandModel', {
        extend: 'Ext.data.Model',
        fields: [
        { name: "category_id", type: "int" },
        { name: "category_name", type: "string" },
        { name: "category_father_id", type: "int" },
        { name: "category_father_name", type: "string" },
        { name: "brand_id", type: "int" },
        { name: "depth", type: "int" }
        ]
    });
    var CateBrandStore = Ext.create('Ext.data.Store', {
        model: 'gigade.cateBrandModel',
        proxy: {
            type: 'ajax',
            url: '/ProductCategory/GetBannerCateBrand',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'item'
            }
        }
    });
    CateBrandStore.on("beforeload", function () {
        Ext.apply(CateBrandStore.proxy.extraParams, {
            banner_cateid: Ext.getCmp('banner_cate_id').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('banner_cate_id').getValue())
        });
    });

    //顯示類別中商品的grid
    var cateBrandGD = new Ext.grid.Panel({
        id: 'cateBrandGD',
        store: CateBrandStore,
        autoScroll: true,
        region: 'center',
        height: 320,
        columnLines: true,
        // frame: true,
        columns: [
        { header: CATEGORYID, dataIndex: 'category_id', width: 65, align: 'center' },
        {
            header: CATEGORYNAME, dataIndex: 'category_name', width: 100, align: 'center'
        },
        { header: "父類別編號", dataIndex: 'category_father_id', width: 65, align: 'center' },
        { header: "父類別名稱", dataIndex: 'category_father_name', width: 100, align: 'center' },
        { header: "品牌編號", dataIndex: 'brand_id', width: 65, align: 'center' },
        //{ header: "品牌名稱", dataIndex: 'brand_name', width: 100, align: 'center' },
        {
            header: "深度", dataIndex: 'depth', width: 55, align: 'center'
        }
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

    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/ProductCategory/SaveCateBrand',
        items: [
        {
            xtype: 'textfield',
            fieldLabel: 'RowID',
            id: 'row_id',
            name: 'row_id',
            submitValue: true,
            hidden: true,
            width: 300
        },
        {
            xtype: 'combobox',
            fieldLabel: BANNERCATID,
            id: 'banner_cate_id',
            name: 'banner_cate_id',
            allowBlank: false,
            editable: false,
            typeAhead: true,
            forceSelection: false,
            queryModel: 'local',
            store: bannerCateFatherStore,
            displayField: 'parameterName',
            valueField: 'parameterCode',
            emptyText: SELECT,
            listeners: {
                select: function (combo, records, eOpts) {
                    CateBrandStore.removeAll();
                    CateBrandStore.load({
                        params: { banner_cateid: Ext.getCmp('banner_cate_id').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('banner_cate_id').getValue()) }
                    ,
                        callback: function () {
                            if (CateBrandStore.data.items.length == 0) {
                                Ext.Msg.alert(INFORMATION, BANNERCATIP);
                            }
                        }
                    });
                }
            }
        },
        cateBrandGD
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: SAVE,
            handler: function () {
                var form = this.up('form').getForm();
                var insertValues = "";
                var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "保存中..." });
                myMask.show();
                if (form.isValid()) {
                    var categoryData = Ext.getCmp("cateBrandGD").store.data.items;
                    if (categoryData.length != 0) {
                        for (var i = 0; i < categoryData.length; i++) {
                            var category_id = categoryData[i].get("category_id");
                            var category_name = categoryData[i].get("category_name");
                            var category_father_id = categoryData[i].get("category_father_id");
                            var category_father_name = categoryData[i].get("category_father_name");
                            var brand = categoryData[i].get("brand_id");
                            var depth = categoryData[i].get("depth");
                            insertValues += category_id + "," + category_name + "," + category_father_id + "," + category_father_name + "," + brand + "," + depth + ";";
                        }
                    }
                    form.submit({
                        params: {
                            insertValues: insertValues,
                            banner_cateid: Ext.getCmp('banner_cate_id').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('banner_cate_id').getValue())
                        },
                        timeout: 60000,
                        success: function (form, action) {
                            myMask.hide();
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SAVESUCCESS);
                                editWin.close();
                                store.load();

                            }
                            else {
                                Ext.Msg.alert(INFORMATION, SAVEFILURE);
                                editWin.close();
                            }
                        },
                        failure: function (form, action) {
                            myMask.hide();
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, SAVEFILURE);
                            editWin.close();
                        }
                    });
                }
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: ADDCATEGORY,
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 500,
        // height: 410,
        y: 100,
        layout: 'fit',
        items: [editFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
        {
            type: 'close',
            qtip: ISCLOSE,
            handler: function (event, toolEl, panel) {
                Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                    if (btn == "yes") {
                        Ext.getCmp('editWin').destroy();
                    }
                    else {
                        return false;
                    }
                });
            }
        }
        ]
    });
    editWin.show();
}
