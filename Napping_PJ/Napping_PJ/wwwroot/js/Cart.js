var cartVue = new Vue({
    el: '#cartVue',
    data: {
        rooms: [],
    },
    mounted: async function () {
        var _this = this;
        //_this.freshCart();
        await _this.getSession();
    },
    methods: {
        addToCart: async function (room) {
            var _this = this;
            //確定已登入
            _this.checkIsLogined();
            //將房間物件加入cookie
            _this.rooms.push(room);
            //console.log(cartList);
            await _this.setSession(_this.rooms);
        },
        removeFromCart: async function (room) {
            var _this = this;
            var cookieName = 'cartItem';
            var idx = _this.rooms.findIndex(r => r.roomId == room.roomId);
            _this.rooms.splice(idx, 1);
            await _this.setSession(_this.rooms);

        },
        setSession: async function (value) {
            var self = this;
            try {
                //console.log(value);
                var response = await axios.post(`${document.location.origin}/cart/SetSession`, value);
                //console.log(response.data);

            } catch (error) {
                console.log(error.response.data);
            }
        },
        getSession: async function () {
            var self = this;
            try {
                var response = await axios.get(`${document.location.origin}/cart/GetSession`);
                //console.log(response.data);
                self.rooms = response.data;
            } catch (error) {
                console.log(error.response.data);
            }
        },
        checkIsLogined: async function () {
            return new Promise((resolve, reject) => {
                 axios.get(`${document.location.origin}/Login/CheckIsLogined`)
                    .then(response => {
                        resolve(response.data);
                    })
                    .catch(error => {
                            console.log(error.response.data);
                            location.href = `${location.origin}${error.response.data}?ReturnUrl=${location.pathname}`;
                        }
                    );
            });
        },
        serviceQuanSub: async function (service) {
            var self = this;
            if (service.serviceQuantity > 0) {
                service.serviceQuantity--;
                await self.setSession(self.rooms);
            }
        },
        serviceQuanAdd: async function (service) {
            var self = this;
            service.serviceQuantity++;
            await self.setSession(self.rooms);
        },
        checkOut: function () {
            var self = this;
            //確定已登入
            self.checkIsLogined();
            //跳轉結帳畫面
            location.href = location.origin + "/CheckOut/Index";
        }
    },
    watch: {
        'rooms': function (newValue) {
            //console.log('Cookie發生變化:', newValue);
        }
    },
    computed: {
        cartTotal: function () {
            var self = this;
            var cartTotal = 0;
            for (var i = 0; i < self.rooms.length; i++) {
                cartTotal += self.rooms[i].totalPrice;
            }
            return cartTotal;
        },
    },
});
