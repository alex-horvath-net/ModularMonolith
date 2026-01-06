using BusinessExperts.Billing;
using BusinessExperts.Order;
using FluentAssertions;
using NetArchTest.Rules;

namespace DevTests.Architecture;

public class ModuleBoundariesTests {
    [Fact]
    public void Common_Must_Not_Depend_On_Modules() {
        var result = Types.InAssembly(typeof(Common.CommonExtensions).Assembly)
            .Should().NotHaveDependencyOnAny("BusinessExperts.Orders", "BusinessExperts.Billing")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(string.Join(Environment.NewLine, result.FailingTypeNames ?? []));
    }

    [Fact]
    public void Orders_Must_Not_Depend_On_Billing() {
        var result = Types.InAssembly(typeof(OrdersExtensions).Assembly)
            .That().ResideInNamespace("BusinessExperts.Orders")
            .Or().ResideInNamespaceMatching("BusinessExperts.Orders\\..*")
            .Should().NotHaveDependencyOnAny("BusinessExperts.Billing")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(string.Join(Environment.NewLine, result.FailingTypeNames ?? []));
    }

    [Fact]
    public void Billing_Must_Not_Depend_On_Orders() {
        var result = Types.InAssembly(typeof(BillingExtensions).Assembly)
            .That().ResideInNamespace("BusinessExperts.Billing")
            .Or().ResideInNamespaceMatching("BusinessExperts.Billing\\..*")
            .Should().NotHaveDependencyOnAny("BusinessExperts.Orders")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(string.Join(Environment.NewLine, result.FailingTypeNames ?? []));
    }

    [Fact]
    public void Modules_Must_Not_Depend_On_Hosts() {
        var orders = Types.InAssembly(typeof(OrdersExtensions).Assembly)
            .That().ResideInNamespace("BusinessExperts.Orders")
            .Or().ResideInNamespaceMatching("BusinessExperts.Orders\\..*")
            .Should().NotHaveDependencyOnAny("ApplicationPortal", "WebApi")
            .GetResult();

        var billing = Types.InAssembly(typeof(BillingExtensions).Assembly)
            .That().ResideInNamespace("BusinessExperts.Billing")
            .Or().ResideInNamespaceMatching("BusinessExperts.Billing\\..*")
            .Should().NotHaveDependencyOnAny("ApplicationPortal", "WebApi")
            .GetResult();

        var failures = new List<string>();
        if (!orders.IsSuccessful) failures.AddRange(orders.FailingTypeNames ?? []);
        if (!billing.IsSuccessful) failures.AddRange(billing.FailingTypeNames ?? []);

        (orders.IsSuccessful && billing.IsSuccessful)
            .Should().BeTrue(string.Join(Environment.NewLine, failures));
    }
}
