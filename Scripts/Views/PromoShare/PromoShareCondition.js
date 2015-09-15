
onConditionClick = function (promo_id) {
    var pageSize = 25;
    Ext.define('gigade.PromoShareCondition', {
        extend: 'Ext.data.Model',
        fields: [
            { name: "promo_id", type: "int" },
            { name: "condition_name", type: "string" },
            { name: "condition_value", type: "string" }
        ]
    });
    var PromoShareConditionStore = Ext.create('Ext.data.Store', {
        autoDestroy: true,
        pageSize: pageSize,
        model: 'gigade.PromoShareCondition',
        proxy: {
            type: 'ajax',
            url: '/PromoShare/PromoShareConditionList',
            reader: {
                type: 'json',
                root: 'data',
                totalProperty: 'totalCount'
            }
        }
        //    autoLoad: true
    });

    PromoShareConditionStore.on('beforeload', function () {
        Ext.apply(PromoShareConditionStore.proxy.extraParams,
            {
                promo_id: promo_id
            });
    });
    PromoShareConditionStore.load();
    var PromoShareConditionGrid = Ext.create('Ext.grid.Panel', {
        id: 'PromoShareConditionGrid',
        title: "",
        store: PromoShareConditionStore,
        sortableColumns: false,
        columnLines: true,
        hidden: false,
        frame: true,
        width: 500,
        height: 450,
        columns: [
            { header: "促銷編號", dataIndex: 'promo_id', width: 150, align: 'center' },
            { header: "條件名稱", dataIndex: 'condition_name', width: 150, align: 'center' },
            { header: "對應值", dataIndex: 'condition_value', width: 150, align: 'center' }
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

    var PromoShareConditionFrm = Ext.create('Ext.form.Panel', {
        width: 500,
        height: 450,
        border: false,
        plain: true,
        defaultType: 'displayfield',
        id: 'ImportFile',
        layout: {
            type: 'hbox'
        },
        items: [PromoShareConditionGrid]
    });
    var teditWins = Ext.create('Ext.window.Window', {
        title: "通用促銷",
        id: 'teditWins',
        width: 500,
        height: 480,
        layout: 'fit',
        items: [PromoShareConditionFrm],
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
                qtip: "關閉",
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp('teditWins').destroy();
                        } else {
                            return false;
                        }
                    });
                }
            }
        ]
    });

    teditWins.show();
}