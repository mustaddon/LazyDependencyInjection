namespace Example;


internal class ContractBase
{ }



internal class ContractA : ContractBase
{}

internal class ContractAB : ContractA
{ }

internal class ContractABC : ContractAB
{ }



internal class ContractB : ContractBase
{}

internal class ContractBA : ContractB
{}