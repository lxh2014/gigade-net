var same = [];
var different = [];

var same_update = [];
var different_update = [];


function ToSame() {
    for (var i = 1, j = product_store.getCount(); i < j; i++) {
        var data = product_store.getAt(0).data;
        product_store.getAt(i).set('item_money', data.item_money);
        product_store.getAt(i).set('event_money', data.event_money);
        product_store.getAt(i).set('item_cost', data.item_cost);
        product_store.getAt(i).set('event_cost', data.event_cost);
    }
}


function CreateGrid(combination) {
    Ext.Ajax.request({
        method: 'post',
        //async:false,
        url: '/ProductCombo/GetMakePrice',
        params: {
            "ProductId": product_id,
            "OldProductId": OLD_PRODUCT_ID
        },
        success: function (response) {
            var resText = eval("(" + response.responseText + ")");
            same = resText.same;
            different = resText.different;

            if (combination == 2 || combination == 3) {
                createNoneGroup();
            }
            else {
                //創建群組grid
                groupPrice();
            }
            Ext.getCmp("price_type").setReadOnly(false);
        },
        failure: function () {
            Ext.Msg.alert(INFORMATION, DATA_LOAD_FAILURE);
        }
    });
}

//創建固定和任選grid
function createNoneGroup() {
    product_store.loadData(same);

    var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
        clicksToEdit: 1,
        listeners: {
            edit: function (editor, e) {
                if (e.colIdex == 4 || e.colIdx == 5 || e.colIdex == 6 || e.colIdx == 7) {
                    if (Ext.getCmp('sameChk').getValue()) {
                        totalPrice(Ext.getCmp('GridPanel'));
                    }
                }
            }
        }
    });
    var PriceGrid = Ext.create("Ext.grid.Panel", {
        id: 'priceGrid',
        width: 700,
        height: 200,
        plugins: [cellEditing],
        store: product_store,
        columns: child_columns,
        tbar: ['->', {
            xtype: 'button',
            text: SAME_WITH_FIRST,
            handler: function () {
                ToSame();
                if (Ext.getCmp('sameChk').getValue()) {
                    totalPrice(Ext.getCmp('GridPanel'));
                }
            }
        }/*, { xtype: 'button', text: '計算最低價、最高價', style: { marginRight: '10px' }, handler: function () { sunMinMaxPrice(Ext.getCmp("GridPanel")) }
        }, { xtype: 'button', text: '計算售價、活動價', handler: function () { totalPrice(Ext.getCmp("GridPanel")) } }*/],
        listeners: {
            beforerender: function () {
                if (combination != 2) {
                    this.title = '<span style="color:red;margin-left:13px">' + COMBO_MUSTBUY + ': ' + product_store.getAt(0).get('G_Must_Buy') + '</span>';
                }
            },
            afterrender: function () {
                var is_same = Ext.getCmp("sameChk").getValue();
                var spec_1 = this.down('*[muiltName=spec_name1]');
                is_same ? spec_1.hide() : spec_1.show();
                var spec_2 = this.down('*[muiltName=spec_name2]');
                is_same ? spec_2.hide() : spec_2.show();
            },
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            },
            viewready: function () {
                window.parent.updateAuth(this, 'muiltName');
            }
        }
    });
    Ext.getCmp("GridPanel").add(PriceGrid);
};


//是否同價 改變時 加載新數據，并關改變grid內展現
function SameChkChange(newValue) {
    var data;
    if (newValue) {
        if (product_id != "" && !isNew) {
            data = same_update;
        }
        else {
            data = same;
        }
    }
    else {
        if (product_id != "" && !isNew) {
            data = different_update;
        }
        else {
            data = different;
        }
    }
    //綁定確定的數據，顯示、隱藏規格欄位
    ChangeStore(data, Ext.getCmp('GridPanel'));
}
//修蓋站臺時查詢并重新綁定站臺價格信息
function GetUpdateData(user_id, user_level, site_id, is_same) {
    if (product_id != "") {
        Ext.getCmp('newPriceSave').setDisabled(true);
        Ext.Ajax.request({
            method: 'post',
            //async: false,
            url: '/ProductCombo/GetUpdatePrice',
            params: {
                "ProductId": product_id,
                "site_id": site_id,
                "user_id": user_id,
                "user_level": user_level
            },
            success: function (response) {
                var resText = eval("(" + response.responseText + ")");
                same_update = resText.same;
                different_update = resText.different;
                SameChkChange(is_same);
                Ext.getCmp('newPriceSave').setDisabled(false);
            },
            failure: function () {
                Ext.Msg.alert(INFORMATION, DATA_LOAD_FAILURE);
            }
        });
    }
};

function ChangeStore(data, panel) {
    var is_same = Ext.getCmp("sameChk").getValue();
    product_store.loadData(data);
    product_store.sort('Pile_Id', 'ASC');
    var group = product_store.getGroups();
    for (var i = 0; i < group.length; i++) {
        var grid = panel.query('grid')[i];
        if (grid) {
            var spec_1 = grid.down('*[muiltName=spec_name1]');
            is_same ? spec_1.hide() : spec_1.show();
            var spec_2 = grid.down('*[muiltName=spec_name2]');
            is_same ? spec_2.hide() : spec_2.show();

            grid.getStore().loadRecords(group[i].children);
        }
    }
}

function change(price_type, is_same,combination) {
    Ext.getCmp("price").setValue(0);
    Ext.getCmp("event_price").setValue(0);

    Ext.getCmp('priceValidTime').show();

    if (price_type == '2') {
        Ext.getCmp("GridPanel").show();

        Ext.getCmp("same_price").show();
        if (is_same) {
            
            if (combination == 2) {
                Ext.getCmp("price1").hide();
                Ext.getCmp("event_price1").hide();

                Ext.getCmp("price").show();
                Ext.getCmp("event_price").show();

                Ext.getCmp("cost").show();
                Ext.getCmp("cost1").hide();
            }
            else if (combination == 3 || combination == 4) {
                Ext.getCmp("price").hide();
                Ext.getCmp("event_price").hide();

                Ext.getCmp("price1").show();
                Ext.getCmp("event_price1").show();
                
                Ext.getCmp("cost").hide();
                Ext.getCmp("cost1").show();
             
            }

            Ext.getCmp("price").setReadOnly(true);
            Ext.getCmp("event_price").setReadOnly(true);

            Ext.getCmp("cost").setReadOnly(true);
            Ext.getCmp("event_cost").setReadOnly(true);
        } else {
            Ext.getCmp("price1").show();
            Ext.getCmp("event_price1").show();

            Ext.getCmp("price").hide();
            Ext.getCmp("event_price").hide();

            Ext.getCmp("cost1").show();
            Ext.getCmp("cost").hide();

        }
    }
    else {
        Ext.getCmp("GridPanel").hide();

        Ext.getCmp("same_price").hide();

        Ext.getCmp("price1").hide();
        Ext.getCmp("event_price1").hide();

        Ext.getCmp("price").show();
        Ext.getCmp("event_price").show();

        Ext.getCmp("price").setReadOnly(false);
        Ext.getCmp("event_price").setReadOnly(false);

        Ext.getCmp("cost").setReadOnly(false); //add by Jiajun  2014.09.25
        Ext.getCmp("event_cost").setReadOnly(false);

        Ext.getCmp("cost").show();
        Ext.getCmp("cost1").hide();
    }
}
