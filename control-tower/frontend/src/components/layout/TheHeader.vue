<template>
  <header class="header">
    <!-- 왼쪽 - 로고 -->
    <div class="header__left">
      <h1 class="logo">
        <router-link to="/" class="navbar-brand">
          <img src="@/assets/snowpipe_logo.png" alt="Snowpipe logo" />
          <span>{{ title }}</span>
        </router-link>
      </h1>
    </div>

    <div class="header__right">
      <span>
        <router-link to="/" class="navbar-brand">
          <span>Server</span>
        </router-link>
        <router-link to="/table" class="navbar-brand">
          <span>Table</span>
        </router-link>
        <router-link to="/tools" class="navbar-brand">
          <span>Tools</span>
        </router-link>
      </span>
    </div>
  </header>
</template>

<script>
export default {
  name: "TheHeader",
  data() {
    return {
      title: process.env.VUE_APP_NAME,
    };
  },
};
</script>

<style lang="scss" scoped>
@import "@/assets/scss/the-header.scss";
</style>
