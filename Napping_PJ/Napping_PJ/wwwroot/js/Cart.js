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
            var self = this;
            //確定已登入
            self.checkIsLogined();
            //將房間物件加入cookie
            try {
                //console.log(value);
                var response = await axios.post(`${document.location.origin}/cart/AddSession`, room);
                //console.log(response.data);
                self.getSession();

            } catch (error) {
                console.log(error.response.data);
            }
        },
        removeFromCart: async function (room) {
            var self = this;
            //確定已登入
            self.checkIsLogined();
            //將房間物件加入cookie
            try {
                //console.log(value);
                var response = await axios.post(`${document.location.origin}/cart/RemoveSession`, room);
                //console.log(response.data);
                await self.getSession();
                if (typeof roomDetailVue !== 'undefined') {
                    //console.log('走起');
                    roomDetailVue.getBookingState(roomDetailVue.RoomDetail.roomId);
                }

            } catch (error) {
                console.log(error.response.data);
            }
        },
        setSession: async function (room) {
            var self = this;
            try {
                //console.log(value);
                var response = await axios.post(`${document.location.origin}/cart/SetSession`, room);
                //console.log(response.data);
                self.getSession();

            } catch (error) {
                console.log(error.response.data);
            }
        },
        getSession: async function () {
            var self = this;
            try {
                var response = await axios.get(`${document.location.origin}/cart/GetSession`);
                console.log(response.data);
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
        serviceQuanSub: async function (room, service) {
            var self = this;
            if (service.serviceQuantity > 0) {
                service.serviceQuantity--;
                await self.setSession(room);
                await self.getSession();
            }
        },
        serviceQuanAdd: async function (room, service) {
            var self = this;
            service.serviceQuantity++;
            await self.setSession(room);
            await self.getSession();
        },
        checkOut: function () {
            var self = this;
            //確定已登入
            self.checkIsLogined();
            //跳轉結帳畫面
            location.href = location.origin + "/CheckOut/Index";

        },
        formateDate: function (dateStr) {
            var date = new Date(dateStr);
            var formattedDate = `${date.getFullYear()}/${(date.getMonth() + 1)
                .toString()
                .padStart(2, "0")}/${date.getDate().toString().padStart(2, "0")}`;
            return formattedDate;
        },
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
                cartTotal += self.rooms[i].tRoomPrice + self.rooms[i].tServicePrice - self.rooms[i].tPromotionPrice;
            }
            return cartTotal;
        },
    },
});
