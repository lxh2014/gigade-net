Ext.define('gigade.PromotionBannerRelation', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "pb_id", type: "int" },
    { name: "brand_id", type: "int" },
    { name: "brand_name", type: "string" }
    ]
});
var pbr_Store = Ext.create('Ext.data.Store', {
    model: 'gigade.PromotionBannerRelation',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/EventPromo/GetRelationList',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
RelationFunction = function (row) {
    pbr_Store.on('beforeload', function () {
        pbr_Store.removeAll();
        Ext.apply(pbr_Store.proxy.extraParams,
        {
            pb_id: row.data.pb_id,
            id: Ext.getCmp("br_id").getValue(),
            name: Ext.getCmp("br_name").getValue()
        });
    });
    var relationFrm = Ext.create('Ext.grid.Panel', {
        id: 'relationList',
        store: pbr_Store,
        anchor: '98%',
        columnLines: true,
        autoScroll: true,
        frame: true,
        columns: [
        { header: '品牌編號', dataIndex: 'brand_id', flex: 1, align: 'center' },
        { header: '品牌名稱', dataIndex: 'brand_name', flex: 2, align: 'center' }
        ],
        tbar: [
        {
            xtype: 'textfield',
            fieldLabel: '品牌編號',
            name: 'br_id',
            id: 'br_id',
            width: 180,
            labelWidth: 60,
            margin: '5 5 0 0',
            regex: /^([0-9]{1,9})$/,
            listeners: {
                specialkey: function (field, e) {
                    if (e.getKey() == e.ENTER) {
                        ListQuery();
                    }
                }
            }
        },
        {
            xtype: 'textfield',
            fieldLabel: '品牌名稱',
            name: 'br_name',
            id: 'br_name',
            width: 180,
            labelWidth: 60,
            margin: '5 5 0 5',
            listeners: {
                specialkey: function (field, e) {
                    if (e.getKey() == e.ENTER) {
                        ListQuery();
                    }
                }
            }
        },
        { xtype: 'button', text: '查詢', id: 'mmQueryBtn', iconCls: 'ui-icon ui-icon-search-2', disabled: false, handler: ListQuery },
        {
            xtype: 'button', text: '重置', id: 'reLoad', iconCls: 'ui-icon ui-icon-reset', handler: function () {
                Ext.getCmp('br_id').reset();
                Ext.getCmp('br_name').reset();
                pbr_Store.load({ params: { pb_id: row.data.pb_id } });
            }
        }
        ]
    });
    var relationWin = Ext.create('Ext.window.Window', {
        title: '促銷品牌',
        iconCls: 'icon-user-edit',
        id: 'relationWin',
        width: 600,
        layout: 'fit',
        height: 400,
        items: [relationFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        listeners: {
            'show': function () {
                pbr_Store.load({ params: { pb_id: row.data.pb_id } });
            }
        }
    });
    relationWin.show();
}

ListQuery = function () {
    pbr_Store.removeAll();
    var id = Ext.getCmp("br_id").getValue();
    var name = Ext.getCmp("br_name").getValue();
    if (!Ext.getCmp('br_id').regex.test(id)) {
        Ext.Msg.alert(INFORMATION, "請輸入有效字符串");
        return;
    }
    if (name != "" && name.trim() == "") {
        Ext.Msg.alert(INFORMATION, "請輸入有效字符串");
        return;
    }
    if (id == "" && name == "") {
        Ext.Msg.alert(INFORMATION, "請輸入查詢條件");
        return;
    }
    else {
        Ext.getCmp("relationList").store.loadPage(1);
    }
}