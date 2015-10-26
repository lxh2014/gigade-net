RelationFunction = function (row) {
    Ext.define('gigade.VendorBrand', {
        extend: 'Ext.data.Model',
        fields: [
        { name: "pb_id", type: "int" },
        { name: "Brand_Id", type: "int" },
        { name: "Brand_Name", type: "string" }
        ]
    });
    var pbr_Store = Ext.create('Ext.data.Store', {
        model: 'gigade.VendorBrand',
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
    pbr_Store.on('beforeload', function () {
        Ext.apply(pbr_Store.proxy.extraParams,
        {
            pb_id: row.data.pb_id,
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
        { header: '品牌編號', dataIndex: 'Brand_Id', flex: 1, align: 'center' },
        { header: '品牌名稱', dataIndex: 'Brand_Name', flex: 2, align: 'center' }
        ]
    });
    var relationWin = Ext.create('Ext.window.Window', {
        title: '促銷品牌',
        iconCls: 'icon-user-edit',
        id: 'relationWin',
        width: 500,
        layout: 'fit',
        height: 400,
        items: [relationFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
        {
            type: 'close',
            qtip: '是否關閉',
            handler: function (event, toolEl, panel) {
                Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                    if (btn == "yes") {
                        Ext.getCmp('relationWin').destroy();
                    }
                    else {
                        return false;
                    }
                });
            }
        }
        ],
        listeners: {
            'show': function () {
                pbr_Store.load({ params: { pb_id: row.data.pb_id } });
            }
        }
    });
    relationWin.show();
}