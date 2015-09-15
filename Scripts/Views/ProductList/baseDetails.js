var baseItems = [
    {
        xtype: 'container',
        layout: 'column',
        id: 'product_brand_name',
        items: [{
            xtype: 'displayfield',
            //hidden: true,
            name: 'brand_name',
            width: 350,
            colName: 'brand_name',
            fieldLabel: BRAND//品牌
        }, {
            xtype: 'displayfield',
            id: 'product_name',
            //hidden: true,
            width: 400,
            colName: 'product_name',
            name: 'Product_Name',
            fieldLabel: PRODUCT_NAME//產品名稱
        }],
        style: { borderBottom: '1px solid #ced9e7' }
    }, {
        xtype: 'container',
        layout: 'column',
        id: 'product_kind_course',
        items: [{
            xtype: 'displayfield',
            width: 350,
            id: 'product_kind',//商品型態
            fieldLabel: PRODUCT_TYPE,
            //hidden: true,
            colName: 'product_kind',
            name: 'product_kind'
        }, {
            xtype: 'displayfield',
            id: 'product_kind_id',
            hidden: true,
            fieldLabel: 'product_kind_id',
            name: 'product_kind_id',
            listeners: {
                change: function () {
                    if (this.value == '1') {
                        Ext.getCmp('ContentPanel').insert(4, Ext.getCmp('courseDetailView'));
                        Ext.getCmp('course_name').show();
                        Ext.getCmp('viewCourse').show();
                    } else {
                        Ext.getCmp('course_name').hide();
                        Ext.getCmp('viewCourse').hide();
                    }
                }
            }
        }, {
            xtype: 'displayfield',
            id: 'course_name',//課程名稱
            hidden: true,
            width: 350,
            fieldLabel: CURRICULUM_NAME,
            name: 'course_name'
        }, {
            xtype: 'displayfield',
            id: 'course_id',
            hidden: true,
            name: 'course_id'
        }, {
            xtype: 'button',
            hidden: true,
            id: 'viewCourse',//查看課程 2015/03/12
            text: CURRICULUM_LOOK_UP,
            handler: function () {
                var course_id = Ext.getCmp("course_id").getValue();
                viewCourse(course_id);
            }
        }],
        style: { borderBottom: '1px solid #ced9e7' }
    }, {
        xtype: 'container',
        layout: 'column',
        id: 'product_sort_vendor',
        items: [{
            xtype: 'displayfield',
            //hidden: true,
            id: 'product_sort',
            fieldLabel: PRODUCT_SORT,//排序
            name: 'product_sort',
            colName: 'product_sort',
            width: 350
        }, {
            xtype: 'displayfield',
            // hidden: true,
            id: 'product_vendor_code',
            fieldLabel: PRODUCT_VENDOR_CODE,//廠商商品編號
            width: 400,
            name: 'product_vendor_code',
            colName: 'product_vendor_code'
        }],
        style: { borderBottom: '1px solid #ced9e7' }
    }, {
        xtype: 'container',
        layout: 'column',
        id: 'product_time',
        items: [{
            xtype: 'displayfield',
            id: 'product_start',
            fieldLabel: PRODUCT_START,//上架時間
            colName: 'product_start',
            name: 'product_start',
            //hidden: true,
            width: 350
        }, {
            xtype: 'displayfield',
            id: 'product_end',
            fieldLabel: PRODUCT_END,//下架時間
            colName: 'product_end',
            width: 400,
            name: 'product_end'
            //hidden: true,
        }],
        style: { borderBottom: '1px solid #ced9e7' }
    }, {
        xtype: 'container',
        layout: 'column',
        id: 'product_expect',
        items: [{
            xtype: 'displayfield',
            id: 'expect_time',
            fieldLabel: EXPECT_TIME,//預計出貨時間
            width: 350,
            //hidden: true,
            name: 'expect_time',
            colName: 'product_end'
        }, {
            xtype: 'displayfield',
            width: 400,
            labelWidth: 110,
            id: 'expect_msg',
            fieldLabel: EXPECT_MSG,//預計出貨信息
            //hidden: true,
            colName: 'expect_msg',
            name: 'expect_msg'
        }],
        style: { borderBottom: '1px solid #ced9e7' }
    }, {
        xtype: 'container',
        layout: 'column',
        id: 'product_mode_extra',
        items: [{
            xtype: 'displayfield',
            //hidden: true,
            id: 'product_freight_set',
            fieldLabel: PRODUCT_FREIGHT_SET, //運送方式
            width: 350,
            name: 'product_freight_set',
            colName: 'product_freight_set'
        }, {
            xtype: 'displayfield',
            //hidden: true,
            id: 'product_mode',
            name: 'product_mode',
            fieldLabel: PRODUCT_MODE,//出貨方式
            width: 400,
            colName: 'product_mode'
        }],
        style: { borderBottom: '1px solid #ced9e7' }
    }, {//add by zhuoqin0830w  2015/03/16
        xtype: 'container',
        layout: 'column',
        id: 'product_deliver_purchase',
        items: [{//供應商出貨天數
            xtype: 'displayfield',
            id: 'deliver_days',
            name: 'deliver_days',
            hidden: true,  //edit by zhuoqin0830w 2015/08/18
            fieldLabel: DELIVER_DAYS,
            //width: 350,
            colName: 'deliver_days'
        }, {//最小採購數量
            xtype: 'displayfield',
            id: 'min_purchase_amount',
            name: 'min_purchase_amount',
            fieldLabel: MIN_PURCHASE_AMOUNT,
            width: 350,
            colName: 'min_purchase_amount'
        }, {
            //安全存量細數
            xtype: 'displayfield',
            id: 'safe_stock_amount',
            name: 'safe_stock_amount',
            fieldLabel: SAFE_STOCK_AMOUNT,
            width: 400,
            colName: 'safe_stock_amount'
        }],
        style: { borderBottom: '1px solid #ced9e7' }
    }, {//add by zhuoqin0830w  2015/03/16
        xtype: 'container',
        layout: 'column',
        id: 'product_safe_tax',
        items: [{
            xtype: 'displayfield',
            //hidden: true,
            id: 'tax_type',
            fieldLabel: TAX_TYPE,
            width: 350,
            name: 'tax_type',
            colName: 'tax_type'
        }, {
            xtype: 'displayfield',
            width: 400,
            id: 'show_in_deliver',//顯示於出貨單中 add by Jiajun 2014/09/16
            fieldLabel: SHOE_IN_DELIVER,
            //hidden: true,
            colName: 'show_in_deliver',
            name: 'show_in_deliver'
        }],
        style: { borderBottom: '1px solid #ced9e7' }
    }, {
        xtype: 'container',
        layout: 'column',
        id: 'product_show_prepaid',
        items: [{
            xtype: 'displayfield',
            width: 350,
            id: 'prepaid',//已買斷的商品
            fieldLabel: PREPAID,
            //hidden: false,
            name: 'prepaid'
        }, {
            xtype: 'displayfield',
            id: 'combination',
            name: 'combination',
            width: 400,
            fieldLabel: COMBINATION,//商品組合類型
            //hidden: true,
            colName: 'combination'
        }],
        style: { borderBottom: '1px solid #ced9e7' }
    }, {
        xtype: 'container',
        layout: 'column',
        id: 'product_combination_process',
        items: [{
            xtype: 'displayfield',
            id: 'combination_id',
            fieldLabel: 'combination_id',
            hidden: true,
            name: 'combination_id',
            listeners: {
                change: function () {
                    if (this.value == '1') {
                        Ext.getCmp('parent_list').show();
                    } else {
                        Ext.getCmp('parent_list').hide();
                    }
                }
            }
        }, {
            xtype: 'displayfield',
            width: 350,
            id: 'process_type',//配送系統
            fieldLabel: PROCESS_TYPE,
            //hidden: true,
            colName: 'process_type',
            name: 'process_type'
        }, {
            xtype: 'displayfield',
            width: 400,
            id: 'sale_name',//販售狀態
            fieldLabel: TRAFFIC_TYPE,
            name: 'sale_name'
            //style: { borderBottom: '1px solid #ced9e7' }
            //hidden: true,
            //colName: 'product_kind',
        }],
        style: { borderBottom: '1px solid #ced9e7' }
    }, {
        xtype: 'displayfield',
        width: 500,
        id: 'parent_list',//單一商品其對應組合商品ID
        fieldLabel: PRODUCT_ASSEMBLY_PARALLELISM_ID,
        colName: 'parent_list',
        name: 'parent_list',
        style: { borderBottom: '1px solid #ced9e7' }
    }, {
        xtype: 'container',
        layout: 'column',
        id: 'purchase_in_advance_con',
        items: [{
            xtype: 'displayfield',
            width: 350,
            id: 'purchase_in_advance',//是否預購
            fieldLabel: IS_PURCHASE_INADVANCE,
            colName: 'purchase_in_advance',
            name: 'purchase_in_advance'
        }, {
            xtype: 'displayfield',
            width: 230,
            id: 'purchase_in_advance_start',
            fieldLabel: PURCHASE_INADVANCE_TIMESE,
            colName: 'purchase_in_advance_start',
            name: 'purchase_in_advance_start'
        }, {
            xtype: 'displayfield',
            value: '~ ',
            id: 'bl',
            margin: '0 5 0 5'
        }, {
            xtype: 'displayfield',
            width: 250,
            id: 'purchase_in_advance_end',
            colName: 'purchase_in_advance_end',
            name: 'purchase_in_advance_end'
        }],
        style: { borderBottom: '1px solid #ced9e7' }
    }]

function viewCourse(course_id) {
    var panel;
    if (PRODUCT_ID) {
        panel = window.parent.parent.parent.Ext.getCmp('ContentPanel')
    } else {
        panel = window.parent.parent.Ext.getCmp('ContentPanel');
    }
    var event = panel.down('#event');
    if (event) {
        event.close();
    }
    var name = Ext.getCmp('course_name').getValue();
    event = panel.add({
        id: 'event',
        title: CURRICULUM_DETAILS + ':' + name,
        html: window.top.rtnFrame("/Course/CurriculDetailView?course_id=" + course_id),
        closable: true
    });
    panel.setActiveTab(event);
    panel.doLayout();
}