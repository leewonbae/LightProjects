<template>
  <div class="body-container">
    <the-header />

    <div class="content-container">
      <div class="router">
        <router-view />
      </div>
    </div>
  </div>
</template>

<script>
import TheHeader from "@/components/layout/TheHeader";

export default {
  name: "OnePage",
  components: {
    TheHeader,
  },
  mounted() {
    document.documentElement.setAttribute("theme", "light");
  },
};
</script>

<style lang="scss">
html,
body {
  margin: 0px !important;
  padding: 0px !important;

  &::-webkit-scrollbar {
    width: 6px;
    background: none;
  }
  &::-webkit-scrollbar-thumb {
    background: #a8a8a8;
    border-radius: 10px;
  }
  &::-webkit-scrollbar-track {
    background: var(--dynamic-login-bg);
  }
}
.body-container {
  position: relative;
  font-family: "NotoSansKR";
  font-size: 0.875rem;
  // min-height: 100vh;
}
.content-container {
  display: flex;
  background-color: var(--dynamic-login-bg);
  min-height: calc(100vh - 58px);

  .sidebar {
    flex: 1;
  }
  .router {
    flex: 5;
    padding: 12px 18px 24px;
    min-width: 600px;
  }
}
</style>
