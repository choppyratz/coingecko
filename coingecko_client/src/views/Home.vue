<template>
  <div class="home">
    <div class="exchanger-base">
      <div class="exchanger">
        <div>
          <b-form-group id="input-group-3" label="Coin 1" label-for="input-1">
            <b-form-select
              id="input-1"
              :options="coinPrices"
              value-field="usd"
              text-field="name"
              v-model="coin1Selected"
              @change="selectOnChange()"
            ></b-form-select>
          </b-form-group>
          <b-input-group>
            <b-form-input v-model="coin1Value" @change="inputChange"></b-form-input>
          </b-input-group>
        </div>
        <div>
          <b-form-group id="input-group-3" label="Coin 2" label-for="input-2">
            <b-form-select
              id="input-2"
              :options="coinPrices"
              value-field="usd"
              text-field="name"
              v-model="coin2Selected"
              @change="selectOnChange()"
            ></b-form-select>
          </b-form-group>
          <b-input-group>
            <b-form-input v-model="coin2Value" disabled></b-form-input>
          </b-input-group>
        </div>
      </div>
    </div>
    <br/>
    <b-table
      striped
      hover
      :items="coinPrices"
      :fields="fields"
      id="coins-table"
    ></b-table>
  </div>
</template>

<script>
// @ is an alias to /src
//import HelloWorld from '@/components/HelloWorld.vue'
import axios from "axios";

export default {
  name: "Home",
  data() {
    return {
      coinPrices: [],
      fields: ["name", "symbol", "usd", "eur", "rub"],
      coin1Value: 0,
      coin2Value: 0,
      coin1Selected: 0,
      coin2Selected: 0,
    };
  },
  mounted() {
    this.getCoinPrices();
    setInterval(this.getCoinPrices, 30000);
  },
  methods: {
    getCoinPrices() {
      axios.get("https://localhost:7086/api/v1/Coingecko").then((response) => {
        this.coinPrices = response.data;
      });
    },
    selectOnChange() {
        this.convertPrices();
    },
    inputChange() {
        this.convertPrices();
    },
    convertPrices() {
        if (this.coin1Selected != 0 && this.coin2Selected != 0 && this.coin1Value >= 0) {
            this.coin2Value = this.coin1Value * this.coin1Selected / this.coin2Selected;
        }
    }
  },
};
</script>

<style scoped>
#coins-table {
  max-width: 600px;
  margin: auto;
}
.exchanger {
  display: flex;
  margin: auto;
}
.exchanger-base {
    display: flex;
    justify-content: center;
}
</style>
