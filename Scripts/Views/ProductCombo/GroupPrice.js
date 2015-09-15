

function groupPrice() {

    var g_panel = Ext.create('Ext.panel.Panel', {
        border: false,
        layout: 'anchor'
    });
    product_store.loadData(same);
    product_store.sort('Pile_Id', 'asc');
    var group = product_store.getGroups();
    if (group.length > 0) {
        for (var i = 0; i < group.length; i++) {
            g_panel.add({
                xtype: 'grid',
                width: 700,
                height: 200,
                title: GROUP + (i + 1),
                store: new Ext.data.Store({
                    model: 'GIGADE.PRODUCT',
                    data: group[i].children
                }),
                tbar: [
                '->',
                { xtype: 'button', text: SAME_WITH_FIRST,
                    handler: function () {
                        var store = this.up('grid').getStore();
                        var rec = store.getAt(0);
                        for (var i = 1; i < store.getCount(); i++) {
                            store.getAt(i).set('item_money', rec.get('item_money'));
                            store.getAt(i).set('event_money', rec.get('event_money'));
                            store.getAt(i).set('item_cost', rec.get('item_cost'));
                            store.getAt(i).set('event_cost', rec.get('event_cost'));
                        }
                        if (Ext.getCmp('sameChk').getValue()) {
                            totalPrice(Ext.getCmp('GridPanel'));
                        }
                    }
                }, { xtype: 'button', text: CALCULATE_MIN_PRICE_AND_MAX_PRICE, handler: function () { sunMinMaxPrice(g_panel) } }/**/],//計算最低價、最高價
                plugins: Ext.create('Ext.grid.plugin.CellEditing', {
                    clicksToEdit: 1,
                    listeners: {
                        beforeedit: function (e, eOpts) {

                        },
                        edit: function (editor, e) {
                            if (e.colIdx == 5 || e.colIdx == 7) {
                                if (Ext.getCmp('sameChk').getValue()) {
                                    totalPrice(Ext.getCmp('GridPanel'));
                                }
                            }
                        }
                    }
                }),
                columns: child_columns,
                listeners: {
                    beforerender: function () {
                        this.title += '<span style="color:red;margin-left:40px">' + MUSTBUY + ':' + this.getStore().getAt(0).get('G_Must_Buy') + '</span>';
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
        }
        Ext.getCmp('GridPanel').add(g_panel);
    }
}

//單一商品規格同價時 計算
function totalPrice(panel) {
    var grids = panel.query('grid');
    var priceSum = 0,
        eventPriceSum = 0;
    //edit by xinglu0624w reason: 组合商品成本为子商品成本总和(同售价).以下有关 costSum,eventCostSum的更改都是xinglu0624w为之
    var costSum = 0,
        eventCostSum = 0;
    maxPrice = 0;   //组合商品价格最大值
    //edit by xinglu0624w end

    for (var i = 0; i < grids.length; i++) {
        var store = grids[i].getStore();
        var mustChoose = Number(store.getAt(0).get('G_Must_Buy')),
            must_buy = 0;
        mustChoose = mustChoose == 0 ? store.getCount() : mustChoose; //群組、任選 g_must_buy不為0，所以為0時為固定組合，此時必須選擇數量=store.getCount()
        store.each(function (rec) {
            if (rec.get('S_Must_Buy') != '0') {
                priceSum += Number(rec.get('item_money')) * Number(rec.get('S_Must_Buy'));
                maxPrice += Number(rec.get('item_money')) * Number(rec.get('S_Must_Buy'));
                eventPriceSum += Number(rec.get('event_money')) * Number(rec.get('S_Must_Buy'));

                costSum += Number(rec.get('item_cost')) * Number(rec.get('S_Must_Buy'));
                eventCostSum += Number(rec.get('event_cost')) * Number(rec.get('S_Must_Buy'));

                must_buy++;
            }
            return true;
        });
        if (must_buy < mustChoose) {
            store.filter(function (rec) { return rec.get('S_Must_Buy') == '0'; })
            if (store.getAt(0).data.Buy_Limit == 1) {//僅限每種一單位
                for (var i = 0, j = mustChoose - must_buy; i < j; i++) {
                    store.sort('item_money'); //售價排序從小到大
                    var record = store.getAt(i);
                    priceSum += Number(record.get('item_money')); //每樣買一件
                    costSum += Number(record.get('item_cost'));
                    store.sort('item_money', 'DESC');
                    maxPrice += Number(store.getAt(i).get('item_money'));

                    store.sort('event_money'); //活動價排序從小到大
                    record = store.getAt(i);
                    eventPriceSum += Number(record.get('event_money')); //每樣買一件
                    eventCostSum += Number(record.get('event_cost'));
                }
            }
            else {
                store.sort('item_money'); //售價排序從小到大
                var record = store.getAt(0);
                priceSum += (mustChoose - must_buy) * Number(record.get('item_money'));
                costSum += (mustChoose - must_buy) * Number(record.get('item_cost'));

                store.sort('item_money', 'DESC');
                maxPrice += (mustChoose - must_buy) * Number(store.getAt(i).get('item_money'));


                store.sort('event_money'); //活動價排序從小到大
                record = store.getAt(i);
                eventPriceSum += (mustChoose - must_buy) * Number(record.get('event_money'));
                eventCostSum += (mustChoose - must_buy) * Number(record.get('event_cost'));
            }
        }
        store.clearFilter(false);
    }
    Ext.getCmp('price').setValue(priceSum);
    //Ext.getCmp('max_price').setValue(maxPriceSum);
    Ext.getCmp('event_price').setValue(eventPriceSum);
    Ext.getCmp('cost').setValue(costSum);
    Ext.getCmp('event_cost').setValue(eventCostSum);
    //alert('售價：' + priceSum + '。活動價：' + eventPriceSum);
}


var minPrice = 0,
    maxPrice = 0,
    maxEventPrice = 0;
//單一商品規格不同價時 計算最低價、最高價
function sunMinMaxPrice(panel) {
    //edit by hufeng0813w reason: 组合商品成本为子商品成本总和(同售价).以下有关 costSum,eventCostSum的更改都是hufeng0813w为之
    var costSum = 0, eventPriceSum = 0,
        eventCostSum = 0;
    //edit by hufeng0813w end
    var tmp;
    minPrice = 0, maxPrice = 0, maxEventPrice = 0;

    var grids = panel.query('grid');
    for (var i = 0; i < grids.length; i++) {
        var store = grids[i].getStore();
        store.group('Child_Id');

        var min = 0, max = 0, maxE = 0;
        //先計算必選的最低價、最高價
        var pro = store.getGroups(); //按商品分組
        var must_choose = Number(store.getAt(0).get('G_Must_Buy')); //群組、任選需選數量
        must_choose = must_choose == 0 ? pro.length : must_choose; //群組、任選 g_must_buy不為0，所以為0時為固定組合，此時必須選擇數量=store里商品數量
        for (var j = 0; j < pro.length; j++) {
            tmp = new Ext.data.Store({
                model: 'GIGADE.PRODUCT'
            });
            tmp.loadRecords(pro[j].children);
            tmp.each(function (rec) {
                rec.set('total_price', Number(rec.get('item_money')) * Number(rec.get('S_Must_Buy')));
                return true;
            });
            tmp.sort('total_price'); //升序 第一筆最小，最後一筆最大
            if (tmp.getAt(0).get('S_Must_Buy') != '0') {
                min += Number(tmp.getAt(0).get('total_price'));
                max += Number(tmp.getAt(tmp.getCount() - 1).get('total_price'));
                //edit by hufeng0813w 2014/06/16
                eventPriceSum += Number(tmp.getAt(0).get('event_money')) * Number(tmp.getAt(0).get('S_Must_Buy'));
                costSum += Number(tmp.getAt(0).get('item_cost')) * Number(tmp.getAt(0).get('S_Must_Buy'));
                eventCostSum += Number(tmp.getAt(0).get('event_cost')) * Number(tmp.getAt(0).get('S_Must_Buy'));
                //edit by hufeng0813w   end
                //必選最大活動價
                tmp.each(function (rec) {
                    rec.set('total_price', Number(rec.get('event_money')) * Number(rec.get('S_Must_Buy')));
                    return true;
                });
                tmp.sort('total_price');
                maxE += Number(tmp.getAt(tmp.getCount() - 1).get('total_price'));

                must_choose--;
            }
        }
        //必選減去后，仍可選的最低價、最高價
        tmp.loadRecords(store.getRange());
        tmp.filter(function (rec) { return rec.get('S_Must_Buy') == '0'; });
        if (tmp.getCount() > 0) {
            if (tmp.getAt(0).data.Buy_Limit == 1) {//僅限一單位勾選
                //最低價 從小到大去 取must_choose的個數,最高價也是,最大活動價 直接取最大的一筆
                for (var i = 0; i < must_choose; i++) {
                    tmp.sort('item_money');
                    min += Number(tmp.getAt(i).get('item_money')); //最低價 從小到大取
                    max += Number(tmp.getAt(tmp.getCount() - (1 + i)).get('item_money')); //最高價 從大到小取
                    //edit by hufeng0813w 2014/06/16 reason:所用單一商品規格不同價時 成本,活動成本,活動價 都需要
                    eventPriceSum += Number(tmp.getAt(i).get('event_money')); //每樣買一件
                    costSum += Number(tmp.getAt(i).get('item_cost'));
                    eventCostSum += Number(tmp.getAt(i).get('event_cost'));
                    //edit by hufeng0813w end 
                    tmp.sort('event_money'); //按照活動價來從小到大排序
                    maxE += Number(tmp.getAt(tmp.getCount() - (i + 1)).get('event_money'));
                }
            }
            else {
                tmp.sort('item_money');
                min += Number(tmp.getAt(0).get('item_money')) * must_choose;
                max += Number(tmp.getAt(tmp.getCount() - 1).get('item_money')) * must_choose;
                //edit by hufeng0813w 2014/06/16 reason:所用單一商品規格不同價時 成本,活動成本,活動價 都需要
                eventPriceSum += Number(tmp.getAt(0).get('event_money')) * must_choose;
                costSum += Number(tmp.getAt(0).get('item_cost')) * must_choose;
                eventCostSum += Number(tmp.getAt(0).get('event_cost')) * must_choose;
                //edit by hufeng0813w end 
                //可選最大活動價
                tmp.sort('event_money');
                maxE += Number(tmp.getAt(tmp.getCount() - 1).get('event_money')) * must_choose;
            }
        }
        //最低價
        //        for (var j = 0; j < must_choose; j++) {
        //            min += Number(tmp.getAt(0).get('item_money'));
        //            tmp.filter(function (rec) { return rec.get('Child_Id') != tmp.getAt(0).get('Child_Id'); });
        //        }
        //最高價
        //        tmp.clearFilter(false);
        //        tmp.filter(function (rec) { return rec.get('S_Must_Buy') == '0'; });
        //        for (var j = 0; j < must_choose; j++) {
        //            max += Number(tmp.getAt(tmp.getCount() - 1).get('item_money'));
        //            tmp.filter(function (rec) { return rec.get('Child_Id') != tmp.getAt(tmp.getCount() - 1).get('Child_Id'); });
        //        }
        tmp.clearFilter(false);

        //ediy by hufeng0813w 2014/06/16 
        Ext.getCmp('event_price').setValue(eventPriceSum);
        Ext.getCmp('cost').setValue(costSum);
        Ext.getCmp('event_cost').setValue(eventCostSum);
        //ediy by hufeng0813w end
        minPrice += min;
        maxPrice += max;
        maxEventPrice += maxE;
        store.loadRecords(tmp.getRange());
        store.clearGrouping();
    }
    alert(MIN_PRICE + '：' + minPrice + '。' + 最高價 + '：' + maxPrice + '。' + MAX_ACTIVITY_PRICE + '：' + maxEventPrice);//最低價,最高價,最高活動價'
}